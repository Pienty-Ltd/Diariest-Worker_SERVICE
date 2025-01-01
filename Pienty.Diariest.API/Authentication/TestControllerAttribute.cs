using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pienty.Diariest.API.Authentication
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestControllerAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token) || token != "test")
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await Task.CompletedTask;
        }
    }
}