using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly ILogger<IAgencyService> _logger;

        public AgencyService(ILogger<IAgencyService> logger)
        {
            _logger = logger;
        }

        public Agency GetAgencyById(long id)
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public Agency GetAgencyByUserId(long userId)
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public bool UpdateAgency(Agency agency)
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public bool AddAgency(Agency agency)
        {
            try
            {

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