using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mscc.GenerativeAI;
using Pienty.Diariest.Core.Configurations;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Services.Handlers;
using ServiceStack;

namespace Pienty.Diariest.Core.Services
{
    public class AIService : IAIService
    {
        private readonly ILogger<IAIService> _logger;
        private readonly GenerativeModel _generativeModel;
        private readonly IRedisService _redisService;

        public AIService(ILogger<IAIService> logger, IOptions<ApplicationConfig> options, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
            _generativeModel = new GenerativeModel()
            {
                ApiKey = options.Value.AIConfig.GeminiAPIKey,
                Model = "gemini-2.0-flash-exp"
            };
        }

        public async Task<APIResponse.SendMessageToGenerativeAIResponse> GenerateContent(string? chatId, string prompt)
        {
            try
            {
                List<ContentResponse> chatHistory = null; 
                if (chatId != null)
                {
                   chatHistory =  _redisService.Get<List<ContentResponse>>(RedisHelper.GetKey_GenerativeAIChat(chatId));
                }
                
                var chat = _generativeModel.StartChat(chatHistory);
                chat.GetId().ToString();
                var response = await chat.SendMessage(prompt);

                Task.Run(() => SaveChatContent(chatId, chatHistory));
                
                return new APIResponse.SendMessageToGenerativeAIResponse()
                {
                    Response = response.Text,
                    ChatId = chat.GetId().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<bool> SaveChatContent(string chatId, List<ContentResponse> response)
        {
            try
            {
                await _redisService.SetAsync<List<ContentResponse>>(RedisHelper.GetKey_GenerativeAIChat(chatId), response, TimeSpan.FromHours(1));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}