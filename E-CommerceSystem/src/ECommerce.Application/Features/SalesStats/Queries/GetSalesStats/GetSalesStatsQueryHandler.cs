
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Abstractions;
using ECommerce.Application.Features.SalesStats.Dtos;
using MediatR;

namespace ECommerce.Application.Features.SalesStats.Queries.GetSalesStats;

public class GetSalesStatsQueryHandler : IRequestHandler<GetSalesStatsQuery, Result<SalesStatsDto>>
{
    private readonly ISalesStatsService _salesStatsService;

    public GetSalesStatsQueryHandler(ISalesStatsService salesStatsService)
    {
        _salesStatsService = salesStatsService;
    }

    public async Task<Result<SalesStatsDto>> Handle(GetSalesStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = await _salesStatsService.GetSalesStatsAsync(
                request.Range,
                request.ForceRefresh,
                cancellationToken);

            var dto = new SalesStatsDto
            {
                TotalUnitsSold = stats.TotalUnitsSold,
                TotalRevenue = stats.TotalRevenue
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<SalesStatsDto>(ex.Message);
        }
    }
}
