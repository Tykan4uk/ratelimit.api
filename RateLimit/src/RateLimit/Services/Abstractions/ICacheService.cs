using System.Threading.Tasks;
using RateLimitApi.Data.Cache;

namespace RateLimitApi.Services.Abstractions
{
    public interface ICacheService<TCacheEntity>
        where TCacheEntity : class, ICacheEntity
    {
        Task AddOrUpdateAsync(TCacheEntity entity);

        Task<TCacheEntity> GetAsync(string name);
    }
}