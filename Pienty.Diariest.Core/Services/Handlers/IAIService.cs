namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IAIService
    {
        Task<string> GenerateContent(string prompt);
    }
}