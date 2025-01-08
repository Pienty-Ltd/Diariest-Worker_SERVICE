using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mscc.GenerativeAI;
using Pienty.Diariest.Core.Configurations;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class AIService : IAIService
    {
        private readonly ILogger<IAIService> _logger;
        private readonly GenerativeModel _generativeModel;

        public AIService(ILogger<IAIService> logger, IOptions<ApplicationConfig> options)
        {
            _logger = logger;
            _generativeModel = new GenerativeModel()
            {
                ApiKey = options.Value.AIConfig.GeminiAPIKey,
                Model = "gemini-2.0-flash-exp"
            };
        }

        public async Task<string> GenerateContent(string prompt)
        {
            try
            {
                var response = await _generativeModel.GenerateContent(prompt);
                return response.Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}