namespace RateLimitApi.Data.Cache
{
    public class RateLimitCacheEntity : ICacheEntity
    {
        public string Name { get; set; } = null!;
        public int Counter { get; set; } = 0;
    }
}