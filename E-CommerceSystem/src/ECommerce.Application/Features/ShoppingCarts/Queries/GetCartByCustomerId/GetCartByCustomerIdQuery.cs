using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Queries.GetCartByCustomerId;

public class GetCartByCustomerIdQuery : IRequest<Result<ShoppingCartDto>>
{
    public Guid CustomerId { get; set; }
}
