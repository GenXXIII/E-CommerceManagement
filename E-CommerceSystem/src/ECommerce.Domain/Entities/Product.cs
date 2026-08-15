using ECommerce.Domain.Abstractions;
using ECommerce.Domain.DomainEvents;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public sealed class Product : AuditableEntity, IAggregateRoot
{
    private readonly List<ProductImage> _images = [];
    private readonly List<ProductReview> _reviews = [];

    private Product()
    {
    }

    public Product(
        Guid categoryId,
        string name,
        string description,
        Price price,
        int quantity)
    {
        CategoryId = categoryId;

        UpdateName(name);

        UpdateDescription(description);

        Price = price;

        UpdateQuantity(quantity);

        Status = ProductStatus.Inactive;
        IsFeatured = false;

        AddDomainEvent(new ProductCreatedEvent(Id, name));
    }

    public Guid CategoryId { get; private set; }

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public Price Price { get; private set; } = default!;

    public int Quantity { get; private set; }

    public ProductStatus Status { get; private set; }

    public bool IsFeatured { get; private set; }

    public Category Category { get; private set; } = default!;

    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        Name = name.Trim();

        MarkUpdated();
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required.");

        Description = description.Trim();

        MarkUpdated();
    }

    public void UpdatePrice(Price price)
    {
        Price = price;

        MarkUpdated();
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative.");

        Quantity = quantity;

        MarkUpdated();
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Invalid quantity.");

        Quantity += quantity;

        MarkUpdated();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Invalid quantity.");

        if (Quantity < quantity)
            throw new DomainException("Insufficient stock.");

        Quantity -= quantity;

        MarkUpdated();
    }

    public void Activate()
    {
        Status = ProductStatus.Active;

        MarkUpdated();
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;

        MarkUpdated();
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
        MarkUpdated();
    }
}
