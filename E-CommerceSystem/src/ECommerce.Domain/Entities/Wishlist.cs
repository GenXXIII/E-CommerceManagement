
using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class Wishlist : AuditableEntity, IAggregateRoot
{
    private readonly List<WishlistItem> _items = [];

    private Wishlist()
    {
    }

    public Wishlist(Guid customerProfileId)
    {
        CustomerProfileId = customerProfileId;
    }

    public Guid CustomerProfileId { get; private set; }

    public CustomerProfile CustomerProfile { get; private set; } = default!;

    public IReadOnlyCollection<WishlistItem> Items => _items.AsReadOnly();

    public void AddItem(Product product)
    {
        if (product.Status != Enums.ProductStatus.Active)
            throw new DomainException("Cannot add inactive product to wishlist.");

        if (_items.Any(x => x.ProductId == product.Id))
            throw new DomainException("Product already in wishlist.");

        _items.Add(new WishlistItem(Id, product.Id));
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Wishlist item not found.");

        _items.Remove(item);
    }

    public void Clear()
    {
        _items.Clear();
    }
}
