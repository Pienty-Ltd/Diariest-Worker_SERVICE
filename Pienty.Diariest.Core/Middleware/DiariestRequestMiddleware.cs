using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Pienty.Diariest.Core.Models.API;

namespace Pienty.Diariest.Core.Middleware
{
    public class DiariestRequestMiddleware
    {
    
        private readonly RequestDelegate _next;

        public DiariestRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post && context.Request.ContentLength == 0)
            {
                var response = new APIResponse.BaseResponse<APIResponse.LoginResponse>
                {
                    Success = false,
                    Error = new APIResponse.ErrorResponse
                    {
                        Message = "Wrong request."
                    }
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                return;
            }

            await _next(context);
        }
    }
}