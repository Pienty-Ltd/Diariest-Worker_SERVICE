using System;
using System.Threading.Tasks;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IRedisService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiry = null);
        bool Remove(string key);
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> RemoveAsync(string key);
    }
}