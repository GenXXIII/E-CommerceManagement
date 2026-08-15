
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Features.Wishlists.Commands.AddToWishlist;

public class AddToWishlistCommand : IRequest<Result<WishlistDto>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
}
