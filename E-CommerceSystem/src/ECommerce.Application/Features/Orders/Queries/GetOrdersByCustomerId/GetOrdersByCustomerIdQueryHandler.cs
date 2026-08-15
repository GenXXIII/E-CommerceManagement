using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersByCustomerId;

public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, Result<List<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByCustomerIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
            var dtos = orders.Adapt<List<OrderDto>>();
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<OrderDto>>(ex.Message);
        }
    }
}
