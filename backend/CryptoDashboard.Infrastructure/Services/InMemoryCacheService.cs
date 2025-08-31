using CryptoDashboard.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoDashboard.Infrastructure.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (object Value, DateTime Expiry)> _cache = new();
        private readonly object _lock = new();

        public Task<T?> GetAsync<T>(string key)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var cached))
                {
                    if (cached.Expiry > DateTime.UtcNow)
                    {
                        return Task.FromResult((T?)cached.Value);
                    }
                    else
                    {
                        _cache.Remove(key);
                    }
                }
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            lock (_lock)
            {
                var expiryTime = DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(5));
                _cache[key] = (value!, expiryTime);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            lock (_lock)
            {
                _cache.Remove(key);
                return Task.CompletedTask;
            }
        }
    }
}