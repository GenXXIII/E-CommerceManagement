
using ECommerce.Application.Features.Products.Dtos;

namespace ECommerce.Application.Features.Wishlists.Dtos;

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public ProductDto Product { get; set; } = default!;
}

public class WishlistDto
{
    public Guid Id { get; set; }
    public Guid CustomerProfileId { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
