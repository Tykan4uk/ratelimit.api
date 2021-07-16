using System.Threading.Tasks;
using RateLimitApi.Configuration;
using RateLimitApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitApi.Models;

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
        public async Task<CheckRateLimitResponse> CheckRateLimitAsync(string name)
        {
            var origin = Request.Headers.TryGetValue("Origin", out var requestOrigin);
            var result = await _rateLimitService.CheckAsync($"{name}{origin}");
            return result;
        }
    }
}