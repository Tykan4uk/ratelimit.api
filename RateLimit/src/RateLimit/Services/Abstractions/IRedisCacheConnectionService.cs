using System;
using StackExchange.Redis;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRedisCacheConnectionService
    {
        public ConnectionMultiplexer Connection { get; }
    }
}