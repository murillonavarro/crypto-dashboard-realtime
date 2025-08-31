using System.Text.Json;
using CryptoDashboard.Core.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CryptoDashboard.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer? _redis;
        private readonly IDatabase? _database;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer? redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _database = _redis?.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_database == null) return default;

            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.HasValue)
                {
                    return JsonSerializer.Deserialize<T>(value!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache key: {key}");
            }
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (_database == null) return;

            try
            {
                var json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, json, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache key: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (_database == null) return;
            await _database.KeyDeleteAsync(key);
        }
    }
}