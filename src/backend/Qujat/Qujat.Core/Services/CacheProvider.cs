using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Qujat.Core.Services
{
    public interface ICacheProvider
    {
        Task<T> GetByKey<T>(string key, bool destroyRecord = false);
        Task SetWithKey<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        Task RemoveByKey(string key);
    }

    public class DefaultCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;
        public DefaultCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T> GetByKey<T>(string key, bool destroyRecord = false)
        {
            var value = _memoryCache.Get<T>(key);

            if (value != null && destroyRecord)
                _memoryCache.Remove(key);

            return Task.FromResult(value);
        }

        public Task RemoveByKey(string key)
        {
            var value = _memoryCache.Get(key);

            if (value != null)
                _memoryCache.Remove(key);

            return Task.CompletedTask;
        }

        public Task SetWithKey<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var absoluteExpirationValue = absoluteExpiration ?? TimeSpan.MaxValue;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteExpirationValue);

            _memoryCache.Set(key, value, cacheEntryOptions);
            return Task.CompletedTask;
        }
    }
}
