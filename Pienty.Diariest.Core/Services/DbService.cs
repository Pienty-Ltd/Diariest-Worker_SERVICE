using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class DbService : IDbService
    {
        private readonly ILogger<IDbService> _logger;
        private readonly IDbConnection _db;
        
        public DbService(ILogger<IDbService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _db = new NpgsqlConnection(configuration.GetConnectionString("PostgreContext"));
        }
        
        public IDbConnection GetDbConnection()
        {
            try
            {
                return _db;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}