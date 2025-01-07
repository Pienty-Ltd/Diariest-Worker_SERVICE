using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pienty.Diariest.Core.Configurations;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Worker.Workers
{
    public class GeneralWorker : IInvocable
    {
        private readonly ILogger<GeneralWorker> _logger;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly IBaseService _baseService;
        private readonly IUserService _userService;
        
        private static object _lock = new object();
        private static bool executing = false;

        public GeneralWorker(ILogger<GeneralWorker> logger, IOptions<ApplicationConfig> options, IBaseService baseService, IUserService userService)
        {
            _logger = logger;
            _options = options;
            _baseService = baseService;
            _userService = userService;
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
                var findedUser = _userService.GetUserWithEmail("tuna@pienty.com");
                _logger.LogError($"{JsonHelper.Serialize(findedUser)}");
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