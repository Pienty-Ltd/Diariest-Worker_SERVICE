using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
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
        var bulkToken = context.HttpContext.Request.Headers["Authorization"];
        var token = StringHelper.GetAuthorizationToken(bulkToken);
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

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

    private bool RemoveIPLimitToken(IRedisService _redisService, ILogger<UserAuthAttribute> _logger, string ipAddress, string token)
    {
        try
        {
            var ipTokenList = _redisService.Get<List<string>>(RedisHelper.GetKey_Limit(ipAddress));
            
            if (ipTokenList == null)
            {
                return true;
            }
            if (ipTokenList.Contains(token))
            {
                ipTokenList.Remove(token);
            }

            if (ipTokenList.Count > 0)
            {
                _redisService.SetAsync(RedisHelper.GetKey_Limit(ipAddress), ipTokenList, TimeSpan.FromMinutes(15));
            }
            else
            {
                _redisService.RemoveAsync(RedisHelper.GetKey_Limit(ipAddress));
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    private bool AddIPLimitToken(IRedisService _redisService, ILogger<UserAuthAttribute> _logger, string ipAddress,
        string token)
    {
        try
        {
            var ipTokenList = _redisService.Get<List<string>>(RedisHelper.GetKey_Limit(ipAddress));
            
            if(ipTokenList == null)
            {
                ipTokenList = new List<string>();
            }

            if (!ipTokenList.Contains(token))
            {
                ipTokenList.Add(token);
            }

            if (ipTokenList.Count > 0)
            {
                _redisService.SetAsync(RedisHelper.GetKey_Limit(ipAddress), ipTokenList, TimeSpan.FromMinutes(15));
            }
            else
            {
                _redisService.RemoveAsync(RedisHelper.GetKey_Limit(ipAddress));
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}