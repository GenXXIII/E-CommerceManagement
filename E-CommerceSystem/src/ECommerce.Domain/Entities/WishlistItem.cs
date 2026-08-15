
using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class WishlistItem : BaseEntity
{
    private WishlistItem()
    {
    }

    internal WishlistItem(Guid wishlistId, Guid productId)
    {
        WishlistId = wishlistId;
        ProductId = productId;
    }

    public Guid WishlistId { get; private set; }

    public Guid ProductId { get; private set; }

    public Wishlist Wishlist { get; private set; } = default!;

    public Product Product { get; private set; } = default!;
}
