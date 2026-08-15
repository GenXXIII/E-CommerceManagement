
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Features.Wishlists.Commands.ClearWishlist;

public class ClearWishlistCommand : IRequest<Result<WishlistDto>>
{
    public Guid CustomerProfileId { get; set; }
}
