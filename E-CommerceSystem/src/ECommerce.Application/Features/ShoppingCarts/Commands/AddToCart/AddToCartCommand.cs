using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Commands.AddToCart;

public class AddToCartCommand : IRequest<Result<ShoppingCartDto>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
