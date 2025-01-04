using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Models.Database.Redis;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Authentication;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class UserAuthAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly UserPermission[] _userPermissions;

    public UserAuthAttribute(params UserPermission[] userPermissions)
    {
        _userPermissions = userPermissions;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            SetUnauthorizedResult(context, "Authorization token is missing. #1", true);
            return;
        }
        
        var serviceProvider = context.HttpContext.RequestServices;
        var redisService = serviceProvider.GetRequiredService<IRedisService>();
        var logger = serviceProvider.GetRequiredService<ILogger<UserAuthAttribute>>();

        try
        {
            var cachedToken = await redisService.GetAsync<AuthenticationToken>(RedisHelper.GetKey_AuthToken(token));
            if (cachedToken == null)
            {
                SetUnauthorizedResult(context, "Authorization token is missing. #2", true);
                return;
            }

            if (cachedToken.Expiration < DateTime.UtcNow)
            {
                if (cachedToken.Expiration > DateTime.UtcNow.AddMinutes(-30))
                {
                    // Token'ı yenile
                    cachedToken.Expiration = DateTime.UtcNow.AddHours(1);
                    await redisService.SetAsync(RedisHelper.GetKey_AuthToken(token), cachedToken, TimeSpan.FromHours(1));
                }
                else
                {
                    SetUnauthorizedResult(context, "Authorization token is missing. #3", true);
                    return;
                }
            }
            
            var user = await redisService.GetAsync<User>(RedisHelper.GetKey_User(cachedToken.UserId));
            if (user == null)
            {
                var userService = serviceProvider.GetRequiredService<IUserService>();
                user = userService.GetUserWithId(cachedToken.UserId);
                if (user == null)
                {
                    SetUnauthorizedResult(context, "User Not Found.", true, statusCode: StatusCodes.Status404NotFound);
                    return;
                }

                await redisService.SetAsync(RedisHelper.GetKey_User(cachedToken.UserId), user, TimeSpan.FromHours(1));
            }

            if (user.permission != UserPermission.Admin && !_userPermissions.Contains(user.permission))
            {
                SetUnauthorizedResult(context, message: "No Access.", false);
                return;
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.email),
                new Claim(ClaimTypes.Role, user.permission.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Token");
            context.HttpContext.User = new ClaimsPrincipal(identity);

            context.HttpContext.Items["AuthToken"] = token;
            context.HttpContext.Items["UserId"] = cachedToken.UserId.ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Authentication Error. #4");
            SetUnauthorizedResult(context, "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.", true);
        }
    }

    private void SetUnauthorizedResult(AuthorizationFilterContext context, string message, bool logout, int statusCode = StatusCodes.Status401Unauthorized)
    {
        context.Result = new ObjectResult(new APIResponse.ErrorResponse { Message = message, Logout = logout })
        {
            StatusCode = statusCode
        };
    }
}