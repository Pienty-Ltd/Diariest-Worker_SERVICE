using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.API.Authentication;
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
        private readonly IAgencyService _agencyService;

        public AgencyController(ILogger<AgencyController> logger, APIMessageService apiMessageService, IRedisService redisService, IAgencyService agencyService)
        {
            _logger = logger;
            _apiMessageService = apiMessageService;
            _redisService = redisService;
            _agencyService = agencyService;
        }

        [TestController]
        [HttpPost("GetAgencyById")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<Agency>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<Agency>),
            StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAgencyById(APIRequest.GetAgencyRequest model)
        {
            try
            {
                var agencyId = model.AgencyId;

                var agency = _agencyService.GetAgencyById(agencyId);
                if (agency == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<Agency>()
                    {
                        Success = false
                    }));
                }

                if (agency.deleted)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<Agency>()
                    {
                        Success = false
                    })); 
                }
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<Agency>()
                {
                    Success = true,
                    Data = agency
                })); 
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