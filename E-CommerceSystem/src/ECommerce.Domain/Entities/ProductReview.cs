using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public sealed class ProductReview : AuditableEntity, IAggregateRoot
{
    private ProductReview()
    {
    }

    public ProductReview(
        Guid customerProfileId,
        Guid productId,
        Guid? orderId,
        int rating,
        string? comment)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        CustomerProfileId = customerProfileId;
        ProductId = productId;
        OrderId = orderId;
        Rating = rating;
        Comment = comment;
        Status = ReviewStatus.Hidden;
    }

    public Guid CustomerProfileId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid? OrderId { get; private set; }

    public int Rating { get; private set; }

    public string? Comment { get; private set; }

    public ReviewStatus Status { get; private set; }

    public CustomerProfile CustomerProfile { get; private set; } = default!;

    public Product Product { get; private set; } = default!;

    public Order? Order { get; private set; }

    public void UpdateReview(int rating, string? comment)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        Rating = rating;
        Comment = comment;
        MarkUpdated();
    }

    public void Hide()
    {
        Status = ReviewStatus.Hidden;
        MarkUpdated();
    }

    public void Show()
    {
        Status = ReviewStatus.Visible;
        MarkUpdated();
    }
}
