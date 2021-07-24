using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RateLimitApi.Data.Cache;
using RateLimitApi.Services;
using RateLimitApi.Services.Abstractions;
using Xunit;

namespace RateLimitApi.UnitTests.Services
{
    public class RateLimitServiceTest
    {
        private readonly IRateLimitService _rateLimitService;
        private readonly Mock<ICacheService<RateLimitCacheEntity>> _cacheService;

        private readonly RateLimitCacheEntity _rateLimitCacheEntitySuccess = new RateLimitCacheEntity()
        {
            Name = "testName"
        };
        private readonly RateLimitCacheEntity _rateLimitCacheEntityFailed = new RateLimitCacheEntity()
        {
        };

        public RateLimitServiceTest()
        {
            _cacheService = new Mock<ICacheService<RateLimitCacheEntity>>();

            _cacheService.Setup(s => s.GetAsync(
                It.Is<string>(i => i.Contains("testName")))).ReturnsAsync(_rateLimitCacheEntitySuccess);

            _cacheService.Setup(s => s.GetAsync(
                It.Is<string>(i => i.Contains("empty")))).ReturnsAsync(_rateLimitCacheEntityFailed);

            _rateLimitService = new RateLimitService(_cacheService.Object);
        }

        [Fact]
        public async Task CheckAsync_Success()
        {
            // arrange
            var testName = "testName";

            // act
            var result = await _rateLimitService.CheckAsync(testName);

            // assert
            result.CheckRateLimit.Should().BeTrue();
        }

        [Fact]
        public async Task CheckAsync_Failed()
        {
            // arrange
            var testName = "testName";

            // act
            var result = await _rateLimitService.CheckAsync(testName);

            // assert
            result.CheckRateLimit.Should().BeTrue();
        }
    }
}
