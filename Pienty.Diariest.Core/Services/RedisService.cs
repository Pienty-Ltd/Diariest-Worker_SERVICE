using System;
using System.Threading.Tasks;
using Pienty.Diariest.Core.Services.Handlers;
using IRedisClientsManager = ServiceStack.Redis.IRedisClientsManager;

namespace Pienty.Diariest.Core.Services
{
    public class RedisService : IRedisService
    {
        private readonly IRedisClientsManager _redisManager;
        
        public RedisService(IRedisClientsManager redisManager)
        {
            _redisManager = redisManager;
        }
        
        public T Get<T>(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return redisClient.Get<T>(key);
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                if (expiry.HasValue)
                {
                    redisClient.Set<T>(key, value, expiry.Value);
                }
                else
                {
                    redisClient.Set<T>(key, value);
                }
            }
        }

        public bool Remove(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return redisClient.Remove(key);
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return await Task.Run(() => redisClient.Get<T>(key));
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                await Task.Run(() =>
                {
                    if (expiry.HasValue)
                    {
                        redisClient.Set<T>(key, value, expiry.Value);
                    }
                    else
                    {
                        redisClient.Set<T>(key, value);
                    }
                });
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return await Task.Run(() => redisClient.Remove(key));
            }
        }
    }
}