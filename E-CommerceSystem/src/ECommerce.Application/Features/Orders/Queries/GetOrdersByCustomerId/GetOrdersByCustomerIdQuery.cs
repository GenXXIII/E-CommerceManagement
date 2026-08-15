using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersByCustomerId;

public class GetOrdersByCustomerIdQuery : IRequest<Result<List<OrderDto>>>
{
    public Guid CustomerId { get; set; }
}
