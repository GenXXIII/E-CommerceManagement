
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly string _instanceName;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
        _instanceName = "ECommerce_";
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var jsonData = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(jsonData))
            return default;

        return JsonSerializer.Deserialize<T>(jsonData, _jsonSerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var jsonData = JsonSerializer.Serialize(value, _jsonSerializerOptions);
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiration.Value;

        await _distributedCache.SetStringAsync(key, jsonData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var database = _connectionMultiplexer.GetDatabase();
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: _instanceName + pattern);

        foreach (var key in keys)
        {
            await database.KeyDeleteAsync(key);
        }

        await Task.CompletedTask;
    }
}
