using System.Threading.Tasks;
using RateLimitApi.Data.Cache;
using RateLimitApi.Models.Responses;
using RateLimitApi.Services.Abstractions;

namespace RateLimitApi.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly ICacheService<RateLimitCacheEntity> _cacheService;

        public RateLimitService(ICacheService<RateLimitCacheEntity> cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<CheckRateLimitResponse> CheckAsync(string name)
        {
            var cache = await _cacheService.GetAsync(name);
            if (cache == null)
            {
                await _cacheService.AddOrUpdateAsync(new RateLimitCacheEntity() { Name = name });
                return new CheckRateLimitResponse { CheckRateLimit = true };
            }

            cache.Counter++;
            await _cacheService.AddOrUpdateAsync(cache);

            if (cache.Counter > 2)
            {
                return new CheckRateLimitResponse { CheckRateLimit = false };
            }

            return new CheckRateLimitResponse { CheckRateLimit = true };
        }
    }
}