
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Features.Wishlists.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommand : IRequest<Result<WishlistDto>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
}
