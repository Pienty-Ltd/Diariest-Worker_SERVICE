using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class LoginHistoryService : ILoginHistoryService
    {
        private readonly ILogger<ILoginHistoryService> _logger;
        private readonly IDbService _dbService;
        
        public LoginHistoryService(
            ILogger<ILoginHistoryService> logger,
            IDbService dbService
            )
        {
            _logger = logger;
            _dbService = dbService;
        }

        public async Task<bool> AddLoginHistory(UserLoginHistory model)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    await conn.InsertAsync(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
        
        public async Task<IEnumerable<UserLoginHistory>> GetUserLoginHistoryAsync(long userId, int page)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    int pageSize = 20;
                    int offset = page * pageSize;

                    var query = @"
                        SELECT * FROM login_history
                        WHERE user_id = @UserId
                        ORDER BY login_date DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    var loginHistory = await conn.QueryAsync<UserLoginHistory>(query, new { UserId = userId, Offset = offset, PageSize = pageSize });
                    return loginHistory;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Enumerable.Empty<UserLoginHistory>();
            }
        }
    }
}