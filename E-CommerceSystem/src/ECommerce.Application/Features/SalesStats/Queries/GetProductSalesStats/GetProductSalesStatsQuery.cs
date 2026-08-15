
using MediatR;
using ECommerce.Domain.Abstractions;
using ECommerce.Application.Features.SalesStats.Dtos;

namespace ECommerce.Application.Features.SalesStats.Queries.GetProductSalesStats;

public class GetProductSalesStatsQuery : IRequest<Result<ProductSalesStatsDto>>
{
    public Guid ProductId { get; set; }
}
