using System;
using System.Threading.Tasks;
using RateLimitApi.Configuration;
using RateLimitApi.Data.Cache;
using StackExchange.Redis;
using RateLimitApi.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimitApi.Services
{
    public class CacheService<TCacheEntity> : ICacheService<TCacheEntity>
            where TCacheEntity : class, ICacheEntity
    {
        private readonly ILogger<CacheService<TCacheEntity>> _logger;
        private readonly IRedisCacheConnectionService _redisCacheConnectionService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly Config _config;

        public CacheService(
            ILogger<CacheService<TCacheEntity>> logger,
            IRedisCacheConnectionService redisCacheConnectionService,
            IOptions<Config> config,
            IJsonSerializer jsonSerializer)
        {
            _logger = logger;
            _redisCacheConnectionService = redisCacheConnectionService;
            _jsonSerializer = jsonSerializer;
            _config = config.Value;
        }

        public Task AddOrUpdateAsync(TCacheEntity entity) => AddOrUpdateInternalAsync(entity);

        public async Task<TCacheEntity> GetAsync(string name)
        {
            var redis = GetRedisDatabase();

            var cacheKey = GetItemCacheKey(name);

            var serialized = await redis.StringGetAsync(cacheKey);

            return !string.IsNullOrEmpty(serialized) ? _jsonSerializer.Deserialize<TCacheEntity>(serialized) : null;
        }

        private string GetItemCacheKey(string name) =>
            $"{name}";

        private async Task AddOrUpdateInternalAsync(TCacheEntity entity, IDatabase redis = null, TimeSpan? expiry = null)
        {
            redis = redis ?? GetRedisDatabase();
            expiry = expiry ?? _config.Redis.CacheTimeout;

            var cacheKey = GetItemCacheKey(entity.Name);
            var serialized = _jsonSerializer.Serialize(entity);

            if (await redis.StringSetAsync(cacheKey, serialized, expiry))
            {
                _logger.LogInformation($"{typeof(TCacheEntity).Name} for {entity.Name} cached. New data: {serialized}");
            }
            else
            {
                _logger.LogInformation($"{typeof(TCacheEntity).Name} for {entity.Name} updated. New data: {serialized}");
            }
        }

        private IDatabase GetRedisDatabase() => _redisCacheConnectionService.Connection.GetDatabase();
    }
}