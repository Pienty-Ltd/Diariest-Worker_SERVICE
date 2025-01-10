using Mscc.GenerativeAI;
using Pienty.Diariest.Core.Models.API;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IAIService
    {
        Task<APIResponse.SendMessageToGenerativeAIResponse> GenerateContent(string? chatId, string prompt);
        Task<bool> SaveChatContent(string chatId, List<ContentResponse> response);
    }
}