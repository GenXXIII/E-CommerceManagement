
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Abstractions;
using ECommerce.Application.Features.SalesStats.Dtos;
using MediatR;

namespace ECommerce.Application.Features.SalesStats.Queries.GetProductSalesStats;

public class GetProductSalesStatsQueryHandler : IRequestHandler<GetProductSalesStatsQuery, Result<ProductSalesStatsDto>>
{
    private readonly ISalesStatsService _salesStatsService;

    public GetProductSalesStatsQueryHandler(ISalesStatsService salesStatsService)
    {
        _salesStatsService = salesStatsService;
    }

    public async Task<Result<ProductSalesStatsDto>> Handle(GetProductSalesStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var unitsSold = await _salesStatsService.GetProductUnitsSoldAsync(request.ProductId, cancellationToken);
            var revenue = await _salesStatsService.GetProductRevenueAsync(request.ProductId, cancellationToken);

            var dto = new ProductSalesStatsDto
            {
                ProductId = request.ProductId,
                UnitsSold = unitsSold,
                Revenue = revenue
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ProductSalesStatsDto>(ex.Message);
        }
    }
}
