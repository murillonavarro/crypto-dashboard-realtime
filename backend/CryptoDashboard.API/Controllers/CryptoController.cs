using Microsoft.AspNetCore.Mvc;
using CryptoDashboard.Core.Interfaces;

namespace CryptoDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;
        private readonly ILogger<CryptoController> _logger;

        public CryptoController(ICryptoService cryptoService, ILogger<CryptoController> logger)
        {
            _cryptoService = cryptoService;
            _logger = logger;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopCryptocurrencies()
        {
            try
            {
                var cryptos = await _cryptoService.GetTopCryptocurrencies();
                return Ok(cryptos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top cryptocurrencies");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetCryptoData(string symbol)
        {
            try
            {
                var data = await _cryptoService.GetCryptoData(symbol);
                if (data == null)
                    return NotFound($"Cryptocurrency {symbol} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting data for {symbol}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{symbol}/history")]
        public async Task<IActionResult> GetPriceHistory(string symbol, [FromQuery] int days = 7)
        {
            try
            {
                var history = await _cryptoService.GetPriceHistory(symbol, days);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting price history for {symbol}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}