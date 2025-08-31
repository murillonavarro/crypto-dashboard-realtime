using System;
using System.Collections.Generic;

namespace CryptoDashboard.Core.Models
{
    public class CryptoData
    {
        public string Id { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal PriceChange24h { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        public decimal MarketCap { get; set; }
        public decimal Volume24h { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<decimal> SparklineData { get; set; } = new();
    }

    public class PricePoint
    {
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
    }
}
