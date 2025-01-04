using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Models.Database.Redis;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/general")]
    public class GeneralController : ControllerBase
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly IUserService _userService;
        private readonly IRedisService _redisService;
        private readonly APIMessageService _apiMessageService;

        public GeneralController(ILogger<GeneralController> logger, IRedisService redisService, IUserService userService, APIMessageService apiMessageService)
        {
            _logger = logger;
            _userService = userService;
            _redisService = redisService;
            _apiMessageService = apiMessageService;
        }
        
        [HttpGet("get")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.GetGeneralResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.GetGeneralResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGeneral()
        {
            try
            {
                var authToken = HttpContext.Items["AuthToken"] as string;
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var cachedAuthorization =
                    _redisService.Get<AuthenticationToken>(RedisHelper.GetKey_AuthToken(authToken));
                var cachedUser = _redisService.Get<User>(RedisHelper.GetKey_User(cachedAuthorization.UserId));
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<User>()
                {
                    Data = null,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<User>()
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