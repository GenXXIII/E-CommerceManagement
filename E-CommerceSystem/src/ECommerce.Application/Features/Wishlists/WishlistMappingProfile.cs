
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.Wishlists;

public class WishlistMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WishlistItem, WishlistItemDto>();
        config.NewConfig<Wishlist, WishlistDto>();
    }
}
