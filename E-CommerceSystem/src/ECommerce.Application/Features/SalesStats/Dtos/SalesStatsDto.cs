
namespace ECommerce.Application.Features.SalesStats.Dtos;

public class SalesStatsDto
{
    public long TotalUnitsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class ProductSalesStatsDto
{
    public Guid ProductId { get; set; }
    public long UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}
