using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly APIMessageService _apiMessageService;

        public UserController(ILogger<UserController> logger, APIMessageService apiMessageService)
        {
            _logger = logger;
            _apiMessageService = apiMessageService;
        }
        
        [HttpPost("Ping")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Ping()
        {
            try
            {
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Data = true,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LogoutResponse>()
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