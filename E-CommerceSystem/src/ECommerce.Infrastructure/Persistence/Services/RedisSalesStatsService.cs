
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Services;

public class RedisSalesStatsService : ISalesStatsService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ApplicationDbContext _context;

    public RedisSalesStatsService(
        IConnectionMultiplexer redis,
        ApplicationDbContext context)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
        _context = context;
    }

    public async Task RecordSuccessfulSaleAsync(Guid productId, int quantitySold, decimal totalRevenue, CancellationToken cancellationToken = default)
    {
        // Use Redis transactions to ensure atomicity
        var transaction = _db.CreateTransaction();
        
        // Increment total units sold
        _ = transaction.StringIncrementAsync("sales:total-units", quantitySold);
        
        // Increment total revenue
        _ = transaction.StringIncrementAsync("sales:total-revenue", (long)(totalRevenue * 100)); // store cents to avoid floating point errors
        
        // Increment product-specific units sold
        _ = transaction.StringIncrementAsync($"sales:product:{productId}:units", quantitySold);
        
        // Increment product-specific revenue
        _ = transaction.StringIncrementAsync($"sales:product:{productId}:revenue", (long)(totalRevenue * 100));

        await transaction.ExecuteAsync();

        // A completed sale changes every dashboard range. Removing these cached
        // snapshots makes the next dashboard request rebuild them from SQL.
        await _db.KeyDeleteAsync(Enum.GetValues<SalesStatsRange>()
            .Select(GetDashboardCacheKey)
            .Select(key => (RedisKey)key)
            .ToArray());
    }

    public async Task<SalesStatsSnapshot> GetSalesStatsAsync(
        SalesStatsRange range,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetDashboardCacheKey(range);

        if (!forceRefresh)
        {
            var cachedValue = await _db.StringGetAsync(cacheKey);
            if (cachedValue.HasValue)
            {
                var cached = JsonSerializer.Deserialize<SalesStatsSnapshot>(cachedValue.ToString());
                if (cached is not null)
                    return cached;
            }
        }

        var paidPayments = _context.Payments
            .AsNoTracking()
            .Where(payment => payment.Status == PaymentStatus.Paid);

        var rangeStart = GetRangeStart(range);
        if (rangeStart.HasValue)
        {
            paidPayments = paidPayments.Where(payment =>
                (payment.UpdatedAt ?? payment.CreatedAt) >= rangeStart.Value);
        }

        var stats = await (
                from item in _context.OrderItems.AsNoTracking()
                join payment in paidPayments on item.OrderId equals payment.OrderId
                select item)
            .GroupBy(_ => 1)
            .Select(group => new SalesStatsSnapshot(
                group.Sum(item => (long)item.Quantity),
                group.Sum(item => item.Quantity * item.UnitPrice)))
            .SingleOrDefaultAsync(cancellationToken)
            ?? new SalesStatsSnapshot(0, 0);

        await _db.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(stats),
            TimeSpan.FromMinutes(5));

        return stats;
    }

    public async Task<long> GetProductUnitsSoldAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync($"sales:product:{productId}:units");
        return value.HasValue ? (long)value : 0;
    }

    public async Task<decimal> GetProductRevenueAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync($"sales:product:{productId}:revenue");
        return value.HasValue ? (decimal)value / 100 : 0;
    }

    private static string GetDashboardCacheKey(SalesStatsRange range) =>
        $"sales:dashboard:{range.ToString().ToLowerInvariant()}";

    private static DateTime? GetRangeStart(SalesStatsRange range)
    {
        var now = DateTime.UtcNow;
        return range switch
        {
            SalesStatsRange.Day => now.Date,
            SalesStatsRange.Month => new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            SalesStatsRange.Year => new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            _ => null
        };
    }
}
