using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IDbService _dbService;

        public UserService(ILogger<UserService> logger, IDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }
        
        public User GetUserWithId(long id)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    string sql = @"select * from user u where u.id = @Id";

                    var res = conn.QueryFirstOrDefault<User>(sql, new { Id = id });

                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public User GetUserWithEmail(string email)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    string sql = @"select * from users u where u.email = @Email";

                    var res = conn.QueryFirstOrDefault<User>(sql, new { Email = email });

                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public bool IsUserExistWithEmail(string email)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    string sql = @"SELECT EXISTS(SELECT 1 FROM users u WHERE u.email = @Email)";

                    var res = conn.QueryFirstOrDefault<bool>(sql, new { Email = email });
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
        
        public bool AddUser(User user)
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    conn.Insert<User>(user);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                bool retval = false;

                using (var conn = _dbService.GetDbConnection())
                {
                    string itemSql = @"select * from user u where u.id = @Id";
                    var item = conn.QueryFirstOrDefault<User>(itemSql, new { Id = user.id });

                    if (item != null)
                    {
                        if (item.permission == UserPermission.Agency && user.permission != UserPermission.Agency)// Agency Yetkisi Değiştirilemez.
                        {
                            item.permission = item.permission;   
                        }
                        else
                        {
                            item.permission = user.permission; 
                        }
                        item.active = user.active;
                        item.deleted = user.deleted;
                        item.phone_number = user.phone_number;
                        
                        conn.Update<User>(item);
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