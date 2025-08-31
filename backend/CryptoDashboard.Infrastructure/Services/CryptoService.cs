using CryptoDashboard.Core.Interfaces;
using CryptoDashboard.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDashboard.Infrastructure.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CryptoService> _logger;

        public CryptoService(ICacheService cacheService, ILogger<CryptoService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<CryptoData>> GetTopCryptocurrencies()
        {
            var random = new Random();
            return await Task.FromResult(new List<CryptoData>
            {
                new() {
                    Id = "bitcoin",
                    Symbol = "BTC",
                    Name = "Bitcoin",
                    CurrentPrice = 45000m + (decimal)(random.NextDouble() * 1000),
                    PriceChange24h = (decimal)(random.NextDouble() * 1000 - 500),
                    PriceChangePercentage24h = (decimal)(random.NextDouble() * 10 - 5),
                    MarketCap = 880000000000m,
                    Volume24h = 28000000000m,
                    LastUpdated = DateTime.UtcNow,
                    SparklineData = Enumerable.Range(0, 24).Select(_ => 45000m + (decimal)(random.NextDouble() * 2000)).ToList()
                },
                new() {
                    Id = "ethereum",
                    Symbol = "ETH",
                    Name = "Ethereum",
                    CurrentPrice = 3000m + (decimal)(random.NextDouble() * 100),
                    PriceChange24h = (decimal)(random.NextDouble() * 100 - 50),
                    PriceChangePercentage24h = (decimal)(random.NextDouble() * 10 - 5),
                    MarketCap = 360000000000m,
                    Volume24h = 15000000000m,
                    LastUpdated = DateTime.UtcNow,
                    SparklineData = Enumerable.Range(0, 24).Select(_ => 3000m + (decimal)(random.NextDouble() * 200)).ToList()
                },
                new() {
                    Id = "cardano",
                    Symbol = "ADA",
                    Name = "Cardano",
                    CurrentPrice = 0.5m + (decimal)(random.NextDouble() * 0.1),
                    PriceChange24h = (decimal)(random.NextDouble() * 0.05 - 0.025),
                    PriceChangePercentage24h = (decimal)(random.NextDouble() * 10 - 5),
                    MarketCap = 16000000000m,
                    Volume24h = 1200000000m,
                    LastUpdated = DateTime.UtcNow,
                    SparklineData = Enumerable.Range(0, 24).Select(_ => 0.5m + (decimal)(random.NextDouble() * 0.1)).ToList()
                }
            });
        }

        public async Task<CryptoData?> GetCryptoData(string symbol)
        {
            var cryptos = await GetTopCryptocurrencies();
            return cryptos.FirstOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<PricePoint>> GetPriceHistory(string symbol, int days)
        {
            var history = new List<PricePoint>();
            var random = new Random();
            var basePrice = symbol.ToUpper() == "BTC" ? 50000m : 3000m;
            
            for (int i = days; i >= 0; i--)
            {
                history.Add(new PricePoint
                {
                    Timestamp = DateTime.UtcNow.AddDays(-i),
                    Price = basePrice + (decimal)(random.NextDouble() * 5000 - 2500)
                });
            }
            
            return await Task.FromResult(history);
        }
    }
}
