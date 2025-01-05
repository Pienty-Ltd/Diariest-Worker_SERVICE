using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/agency")]
    public class AgencyController : ControllerBase
    {
        private readonly ILogger<AgencyController> _logger;
        private readonly APIMessageService _apiMessageService;
        private readonly IRedisService _redisService;

        public AgencyController(ILogger<AgencyController> logger, APIMessageService apiMessageService, IRedisService redisService)
        {
            _logger = logger;
            _apiMessageService = apiMessageService;
            _redisService = redisService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.LoginResponse>),
            StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(APIRequest.LoginRequest model)
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                {
                    Success = false,
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