
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Features.Wishlists.Queries.GetWishlistByCustomerId;

public class GetWishlistByCustomerIdQuery : IRequest<Result<WishlistDto>>
{
    public Guid CustomerId { get; set; }
}
