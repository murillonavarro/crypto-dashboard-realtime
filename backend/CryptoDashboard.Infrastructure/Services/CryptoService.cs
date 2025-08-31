using CryptoDashboard.Core.Interfaces;
using CryptoDashboard.Core.Models;
using Microsoft.Extensions.Logging;

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
            // Por enquanto retorna dados mock
            var random = new Random();
            return new List<CryptoData>
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
                }
            };
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
            var basePrice = 50000m;
            
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
