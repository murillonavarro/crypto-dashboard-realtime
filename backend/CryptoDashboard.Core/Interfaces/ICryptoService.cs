using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoDashboard.Core.Models;

namespace CryptoDashboard.Core.Interfaces
{
    public interface ICryptoService
    {
        Task<List<CryptoData>> GetTopCryptocurrencies();
        Task<CryptoData?> GetCryptoData(string symbol);
        Task<List<PricePoint>> GetPriceHistory(string symbol, int days);
    }
}
