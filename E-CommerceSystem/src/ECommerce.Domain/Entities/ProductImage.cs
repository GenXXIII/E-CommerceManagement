using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class ProductImage : AuditableEntity
{
    private ProductImage()
    {
    }

    public ProductImage(Guid productId, string imageUrl)
    {
        ProductId = productId;

        SetImage(imageUrl);
    }

    public Guid ProductId { get; private set; }

    public string ImageUrl { get; private set; } = default!;

    public Product Product { get; private set; } = default!;

    public void SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL is required.");

        ImageUrl = imageUrl.Trim();

        MarkUpdated();
    }
}