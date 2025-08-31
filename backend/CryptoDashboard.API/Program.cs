using CryptoDashboard.API.Hubs;
using CryptoDashboard.API.Services;
using CryptoDashboard.Core.Interfaces;
using CryptoDashboard.Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    try
    {
        var redis = ConnectionMultiplexer.Connect(configuration);
        Console.WriteLine("✅ Redis conectado com sucesso!");
        return redis;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Redis não disponível: {ex.Message}");
        Console.WriteLine("📌 Usando cache em memória...");
        return null!;
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Cache Service
builder.Services.AddSingleton<ICacheService>(sp =>
{
    var redis = sp.GetService<IConnectionMultiplexer>();

    if (redis != null)
    {
        var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();
        return new RedisCacheService(redis, logger);
    }
    else
    {
        return new InMemoryCacheService();
    }
});

// Services
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddHostedService<CryptoPriceUpdateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.MapHub<CryptoHub>("/hubs/crypto");

Console.WriteLine("🚀 Crypto Dashboard API iniciada!");
Console.WriteLine("📊 Swagger disponível em: http://localhost:5178/swagger");
Console.WriteLine("🔌 SignalR Hub disponível em: /hubs/crypto");

app.Run();