
using ECommerce.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Services;

public class RedisOrderStatusNotifier : IOrderStatusNotifier
{
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisOrderStatusNotifier(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task NotifyOrderStatusChangedAsync(Guid orderId, string status, CancellationToken cancellationToken = default)
    {
        var subscriber = _redis.GetSubscriber();
        var message = JsonSerializer.Serialize(new
        {
            OrderId = orderId,
            Status = status,
            Timestamp = DateTime.UtcNow
        }, _jsonOptions);
        
        await subscriber.PublishAsync(RedisChannel.Literal("order-status-updates"), message);
    }
}

