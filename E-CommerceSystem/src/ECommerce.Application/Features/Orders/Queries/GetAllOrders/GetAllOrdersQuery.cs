using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetAllOrders;

public sealed record GetAllOrdersQuery : IRequest<Result<List<OrderDto>>>;

public sealed class GetAllOrdersQueryHandler(IOrderRepository orders)
    : IRequestHandler<GetAllOrdersQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await orders.GetAllAsync(cancellationToken);
        return Result.Success(result.Adapt<List<OrderDto>>());
    }
}
