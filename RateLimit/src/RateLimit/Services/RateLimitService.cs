using System.Threading.Tasks;
using RateLimitApi.Data.Cache;
using RateLimitApi.Models;
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

        public async Task<bool> CheckAsync(string name)
        {
            var cache = await _cacheService.GetAsync(name);
            if (cache == null)
            {
                await _cacheService.AddOrUpdateAsync(new RateLimitCacheEntity() { Name = name });
                return true;
            }

            cache.Counter++;

            if (cache.Counter > 2)
            {
                return false;
            }

            await _cacheService.AddOrUpdateAsync(cache);
            return true;
        }
    }
}