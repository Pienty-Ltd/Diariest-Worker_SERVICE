using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.API.Authentication;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/gai")]
    public class GenerativeAIController : ControllerBase
    {
        private readonly ILogger<GenerativeAIController> _logger;
        private readonly APIMessageService _apiMessageService;
        private readonly IAIService _aiService;
        private readonly IRedisService _redisService;
        
        public GenerativeAIController(ILogger<GenerativeAIController> logger, APIMessageService apiMessageService, IAIService aiService, IRedisService redisService)
        {
            _logger = logger;
            _apiMessageService = apiMessageService;
            _aiService = aiService;
            _redisService = redisService;
        }
        
        [HttpGet("send")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.SendMessageToGenerativeAIResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.SendMessageToGenerativeAIResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessageToGenerativeAI(APIRequest.SendMessageToGenerativeAIRequest model) 
        {
            try
            {
                var prompt = model.Prompt;

                var response = await _aiService.GenerateContent(prompt);
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.SendMessageToGenerativeAIResponse>()
                {
                    Data = null,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.SendMessageToGenerativeAIResponse>()
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