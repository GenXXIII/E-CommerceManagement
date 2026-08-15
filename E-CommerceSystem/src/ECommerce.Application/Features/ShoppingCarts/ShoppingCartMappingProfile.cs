using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.ShoppingCarts;

public class ShoppingCartMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CartItem, CartItemDto>();
        config.NewConfig<ShoppingCart, ShoppingCartDto>();
    }
}
