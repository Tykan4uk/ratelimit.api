using StackExchange.Redis;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRedisCacheConnectionService
    {
        public IConnectionMultiplexer Connection { get; }
    }
}