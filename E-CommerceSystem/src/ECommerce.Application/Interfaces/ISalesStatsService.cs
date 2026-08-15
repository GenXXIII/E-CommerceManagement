
namespace ECommerce.Application.Interfaces;

public enum SalesStatsRange
{
    Day,
    Month,
    Year,
    Overall
}

public sealed record SalesStatsSnapshot(long TotalUnitsSold, decimal TotalRevenue);

public interface ISalesStatsService
{
    Task RecordSuccessfulSaleAsync(Guid productId, int quantitySold, decimal totalRevenue, CancellationToken cancellationToken = default);
    Task<SalesStatsSnapshot> GetSalesStatsAsync(SalesStatsRange range, bool forceRefresh = false, CancellationToken cancellationToken = default);
    Task<long> GetProductUnitsSoldAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<decimal> GetProductRevenueAsync(Guid productId, CancellationToken cancellationToken = default);
}
