using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Commands.RemoveFromCart;

public class RemoveFromCartCommand : IRequest<Result<ShoppingCartDto>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
}
