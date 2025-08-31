using Microsoft.AspNetCore.SignalR;
using CryptoDashboard.Core.Interfaces;

namespace CryptoDashboard.API.Hubs
{
    public class CryptoHub : Hub
    {
        private readonly ICryptoService _cryptoService;
        private readonly ILogger<CryptoHub> _logger;

        public CryptoHub(ICryptoService cryptoService, ILogger<CryptoHub> logger)
        {
            _cryptoService = cryptoService;
            _logger = logger;
        }

        public async Task Subscribe(string symbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"crypto-{symbol.ToLower()}");
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to {symbol}");
            
            var currentData = await _cryptoService.GetCryptoData(symbol);
            if (currentData != null)
            {
                await Clients.Caller.SendAsync("PriceUpdate", currentData);
            }
        }

        public async Task Unsubscribe(string symbol)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"crypto-{symbol.ToLower()}");
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}
