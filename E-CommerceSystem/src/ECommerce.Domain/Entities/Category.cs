using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class Category : AuditableEntity, IAggregateRoot
{
    private readonly List<Product> _products = [];

    private Category()
    {
    }

    public Category(string name, string? description)
    {
        UpdateName(name);
        Description = description;
        IsActive = false;
    }

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public string? ImageUrl { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        Name = name.Trim();

        MarkUpdated();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();

        MarkUpdated();
    }

    public void SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL is required.");

        ImageUrl = imageUrl.Trim();
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;

        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;

        MarkUpdated();
    }
}
