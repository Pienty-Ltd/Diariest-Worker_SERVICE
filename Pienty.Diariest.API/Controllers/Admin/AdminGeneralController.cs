using Microsoft.AspNetCore.Mvc;

namespace Pienty.Diariest.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/general")]
    public class AdminGeneralController : ControllerBase
    {
        private readonly ILogger<AdminGeneralController> _logger;

        public AdminGeneralController(ILogger<AdminGeneralController> logger)
        {
            _logger = logger;
        }
    }
}