using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderCommand : IRequest<Result<Guid>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid AddressId { get; set; }
    public string? Note { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
