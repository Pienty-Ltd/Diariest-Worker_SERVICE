using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Services
{
    public class PageService : IPageService
    {
        private readonly ILogger<IPageService> _logger;
        private readonly IDbService _dbService;

        public PageService(ILogger<IPageService> logger, IDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }
    }
}