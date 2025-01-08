using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pienty.Diariest.Core.Configurations;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Worker.Workers
{
    public class CacheableWorker : IInvocable
    {
        private readonly ILogger<GeneralWorker> _logger;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly IBaseService _baseService;
        private readonly IPageService _pageService;
        
        private static object _lock = new object();
        private static bool executing = false;

        public CacheableWorker(ILogger<GeneralWorker> logger, IOptions<ApplicationConfig> options, IBaseService baseService, IPageService pageService)
        {
            _logger = logger;
            _options = options;
            _baseService = baseService;
            _pageService = pageService;
        }
        
        public Task Invoke()
        {
            if (executing)
            {
                lock (_lock)
                {
                    if (executing)
                    {
                        return Task.CompletedTask;
                    }
                }
            }

            executing = true;

            try
            {
                _pageService.InitializeAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            
            executing = false;

            return Task.CompletedTask;
        }
    }
}