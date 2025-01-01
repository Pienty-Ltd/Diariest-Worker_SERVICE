using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class BaseService : IBaseService
    {
        private readonly ILogger<IBaseService> _logger;

        public BaseService(ILogger<IBaseService> logger)
        {
            _logger = logger;
        }

    }
}