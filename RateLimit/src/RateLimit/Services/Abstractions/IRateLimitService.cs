using System.Threading.Tasks;
using RateLimitApi.Models;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRateLimitService
    {
        Task<bool> CheckAsync(string name);
    }
}