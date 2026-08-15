using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<Result<OrderDto>>
{
    public Guid Id { get; set; }
}
