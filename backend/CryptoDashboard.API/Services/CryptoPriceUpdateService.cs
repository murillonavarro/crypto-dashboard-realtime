using Microsoft.AspNetCore.SignalR;
using CryptoDashboard.API.Hubs;
using CryptoDashboard.Core.Interfaces;

namespace CryptoDashboard.API.Services
{
    public class CryptoPriceUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<CryptoHub> _hubContext;
        private readonly ILogger<CryptoPriceUpdateService> _logger;

        public CryptoPriceUpdateService(
            IServiceProvider serviceProvider,
            IHubContext<CryptoHub> hubContext,
            ILogger<CryptoPriceUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoService>();
                    var topCryptos = await cryptoService.GetTopCryptocurrencies();
                    
                    foreach (var crypto in topCryptos)
                    {
                        await _hubContext.Clients.Group($"crypto-{crypto.Symbol.ToLower()}")
                            .SendAsync("PriceUpdate", crypto);
                    }
                    
                    await _hubContext.Clients.All.SendAsync("MarketUpdate", topCryptos);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating prices");
                }
                
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
