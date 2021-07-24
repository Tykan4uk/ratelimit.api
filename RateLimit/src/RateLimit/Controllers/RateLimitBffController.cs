using System.Threading.Tasks;
using RateLimitApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimitApi.Models.Responses;
using RateLimitApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;

namespace RateLimitApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    [Authorize(Policy = "ApiScopeBff")]
    public class RateLimitBffController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IRateLimitService _rateLimitService;

        public RateLimitBffController(
            ILogger<ManageController> logger,
            IRateLimitService rateLimitService)
        {
            _logger = logger;
            _rateLimitService = rateLimitService;
        }

        [HttpPost]
        public async Task<CheckRateLimitResponse> CheckRateLimitAsync([FromBody] CheckRateLimitRequest request)
        {
            Request.Headers.TryGetValue("Origin", out var requestOrigin);
            var result = await _rateLimitService.CheckAsync($"{request.Name}{requestOrigin}");

            if (!result.CheckRateLimit)
            {
                _logger.LogInformation("(RateLimitBffController/CheckRateLimitAsync) Too many request!");
            }

            return result;
        }
    }
}