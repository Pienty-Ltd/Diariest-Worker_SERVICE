using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Pienty.Diariest.Core.Configurations;

namespace Pienty.Diariest.API.Authentication
{
    public class TestControllerFilter : IAsyncAuthorizationFilter
    {
        private readonly IOptions<ApplicationConfig> _options;

        public TestControllerFilter(IOptions<ApplicationConfig> options)
        {
            _options = options;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["TestToken"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token) || token != _options.Value.ServerConfig.TestToken)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestControllerAttribute : TypeFilterAttribute
    {
        public TestControllerAttribute() : base(typeof(TestControllerFilter))
        {
        }
    }
}