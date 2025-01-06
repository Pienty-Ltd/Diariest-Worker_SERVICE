using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IAgencyService
    {
        Agency GetAgencyById(long id);
        Agency GetAgencyByUserId(long userId);
        bool UpdateAgency(Agency agency);
        bool AddAgency(Agency agency);
    }
}