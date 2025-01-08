using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class PageService : IPageService
    {
        private readonly ILogger<IPageService> _logger;
        private readonly IDbService _dbService;
        private readonly IRedisService _redisService;

        public PageService(ILogger<IPageService> logger, IDbService dbService, IRedisService redisService)
        {
            _logger = logger;
            _dbService = dbService;
            _redisService = redisService;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await CacheAppPagesToRedisAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        
        public async Task CacheAppPagesToRedisAsync()
        {
            try
            {
                using (var conn = _dbService.GetDbConnection())
                {
                    var appPages = await conn.GetAllAsync<AppPage>();

                    var appPageDict = appPages.ToDictionary(page => page.id);
                    foreach (var page in appPages)
                    {
                        page.Children ??= new List<AppPage>();

                        if (page.parent_id != 0 && page.parent_id != -1 && appPageDict.ContainsKey(page.parent_id))
                        {
                            var parentPage = appPageDict[page.parent_id];
                            parentPage.Children ??= new List<AppPage>();
                            parentPage.Children.Add(page);
                            
                            page.endpoint = Path.Combine(parentPage.endpoint ?? "", page.endpoint ?? "").Replace("\\", "/");
                        }

                    }

                    var topLevelAppPages = appPages.Where(page => page.parent_id == 0 || page.parent_id == -1).ToList();
                    var jsonData = JsonHelper.Serialize(topLevelAppPages);
                    

                    await _redisService.SetAsync(RedisHelper.GetKey_AppPages(), jsonData);

                    _logger.LogInformation("App pages data cached to Redis successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching app pages data to Redis.");
            }
        }
    }
}