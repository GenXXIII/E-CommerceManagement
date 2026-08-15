
using MediatR;
using ECommerce.Domain.Abstractions;
using ECommerce.Application.Features.SalesStats.Dtos;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Features.SalesStats.Queries.GetSalesStats;

public class GetSalesStatsQuery : IRequest<Result<SalesStatsDto>>
{
    public SalesStatsRange Range { get; set; } = SalesStatsRange.Overall;
    public bool ForceRefresh { get; set; }
}
