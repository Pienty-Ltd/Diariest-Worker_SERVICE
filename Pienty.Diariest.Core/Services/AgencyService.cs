using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Middleware.Attributes;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly ILogger<IAgencyService> _logger;
        private readonly IDbService _dbService;
        
        public AgencyService(ILogger<IAgencyService> logger, IDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        [Cacheable(60)]
        public Agency GetAgencyById(long id)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    string sql = @"select * from agency a where a.id = @Id";

                    var res = conn.QueryFirstOrDefault<Agency>(sql, new { Id = id });

                    return res;
                }
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
                //TODO
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public bool AddAgency(Agency agency)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    conn.Insert<Agency>(agency);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public bool UpdateAgency(Agency model)
        {
            try
            {
                bool retval = false;

                using (var conn = _dbService.GetDbConnection())
                {
                    string sql = @"select * from agency a where a.id = @Id";
                    var item = conn.QueryFirstOrDefault<Agency>(sql, new { Id = model.id });

                    if (item != null)
                    {
                        item.name = model.name;
                        item.email = model.email;
                        item.active = model.active;
                        item.language = model.language;

                        if (model.deleted && (item.deleted != model.deleted))
                        {
                            item.deleted_date = DateTime.UtcNow;
                            item.deleted = true;
                        }
                        
                        item.updated_date = DateTime.UtcNow;
                        retval = conn.Update<Agency>(item);
                    }
                    else
                    {
                        retval = false;
                    }
                }
                return retval;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}