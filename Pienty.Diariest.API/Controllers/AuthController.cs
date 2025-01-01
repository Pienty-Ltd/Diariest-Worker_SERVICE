using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("")]
    public class AuthController
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        
        
    }
}