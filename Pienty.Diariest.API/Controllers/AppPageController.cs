using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/page")]
    public class AppPageController : ControllerBase
    {
        private readonly ILogger<AppPageController> _logger;
        private readonly APIMessageService _apiMessageService;
        private readonly IRedisService _redisService;

        public AppPageController(
            ILogger<AppPageController> logger, 
            APIMessageService apiMessageService, 
            IRedisService redisService
            )
        {
            _logger = logger;
            _apiMessageService = apiMessageService;
            _redisService = redisService;
        }
        
        //[UserAuth(UserPermission.Admin, UserPermission.Agency, UserPermission.Client)]
        [HttpGet("get")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<List<AppPage>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<List<AppPage>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPages() 
        {
            try
            {
                var cachedPages = JsonHelper.Deserialize<List<AppPage>>(_redisService.Get<string>(RedisHelper.GetKey_AppPages()));

                if (cachedPages == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<List<AppPage>>()
                    {
                        Data = null,
                        Message = "Cached pages not found.",
                        Success = false
                    }));
                }
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<List<AppPage>>()
                {
                    Data = cachedPages,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<List<AppPage>>()
                {
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Message = ex.Message
                    }
                }));
            }
        }
    }
}