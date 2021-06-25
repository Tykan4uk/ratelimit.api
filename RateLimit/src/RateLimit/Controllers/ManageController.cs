using System.Threading.Tasks;
using RateLimitApi.Configuration;
using RateLimitApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimitApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IRateLimitService _rateLimitService;
        private readonly Config _config;

        public ManageController(
            ILogger<ManageController> logger,
            IOptions<Config> config,
            IRateLimitService rateLimitService)
        {
            _logger = logger;
            _rateLimitService = rateLimitService;
            _config = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> CheckRateLimitAsync(string name)
        {
            var result = await _rateLimitService.CheckAsync(name);
            return result == true ? Ok() : StatusCode(409);
        }
    }
}