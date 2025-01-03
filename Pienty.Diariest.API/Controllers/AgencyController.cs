using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/agency")]
    public class AgencyController : ControllerBase
    {
        private readonly ILogger<AgencyController> _logger;

        public AgencyController(ILogger<AgencyController> logger)
        {
            _logger = logger;
        }

    }
}