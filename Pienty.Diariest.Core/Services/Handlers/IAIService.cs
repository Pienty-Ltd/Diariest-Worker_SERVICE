using Mscc.GenerativeAI;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IAIService
    {
        Task<string> GenerateContent(string? chatId, string prompt);
        Task<bool> SaveChatContent(string chatId, List<ContentResponse> response);
    }
}