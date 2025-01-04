using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class DbService : IDbService, IDisposable
    {
        private readonly ILogger<IDbService> _logger;
        private readonly string _connectionString;
        private readonly NpgsqlDataSource _dataSource;

        public DbService(ILogger<IDbService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("PostgreContext");
            
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            _dataSource = dataSourceBuilder.Build();
        }

        public IDbConnection GetDbConnection()
        {
            try
            {
                var connection = _dataSource.CreateConnection();
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veritabanı bağlantısı açılırken hata oluştu");
                throw;
            }
        }

        public void Dispose()
        {
            _dataSource?.Dispose();
        }
    }
}