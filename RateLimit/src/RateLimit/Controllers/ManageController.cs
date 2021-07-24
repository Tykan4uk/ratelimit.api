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
    [Authorize(Policy = "ApiScope")]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IRateLimitService _rateLimitService;

        public ManageController(
            ILogger<ManageController> logger,
            IRateLimitService rateLimitService)
        {
            _logger = logger;
            _rateLimitService = rateLimitService;
        }

        [HttpGet]
        public async Task<CheckRateLimitResponse> CheckRateLimitAsync([FromQuery] CheckRateLimitRequest request)
        {
            var origin = Request.Headers.TryGetValue("Origin", out var requestOrigin);
            var result = await _rateLimitService.CheckAsync($"{request.Name}{origin}");

            if (!result.CheckRateLimit)
            {
                _logger.LogInformation("(ManageController/CheckRateLimitAsync) Too many request!");
            }

            return result;
        }
    }
}