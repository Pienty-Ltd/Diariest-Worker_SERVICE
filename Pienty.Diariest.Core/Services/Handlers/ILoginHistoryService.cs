using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface ILoginHistoryService
    {
        Task<bool> AddLoginHistory(UserLoginHistory model);
    }
}