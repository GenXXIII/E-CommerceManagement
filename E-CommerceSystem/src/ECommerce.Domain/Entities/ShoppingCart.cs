using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class ShoppingCart : AuditableEntity, IAggregateRoot
{
    private readonly List<CartItem> _items = [];

    private ShoppingCart()
    {
    }

    public ShoppingCart(Guid customerProfileId)
    {
        CustomerProfileId = customerProfileId;
    }

    public Guid CustomerProfileId { get; private set; }

    public CustomerProfile CustomerProfile { get; private set; } = default!;

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(x => x.TotalPrice);

    public void AddItem(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (product.Status != Enums.ProductStatus.Active)
            throw new DomainException("Cannot add inactive product to cart.");

        var existing = _items.FirstOrDefault(x => x.ProductId == product.Id);

        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new CartItem(Id, product.Id, quantity, product.Price));
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Cart item not found.");

        _items.Remove(item);
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Cart item not found.");

        item.UpdateQuantity(quantity);
    }

    public void Clear()
    {
        _items.Clear();
    }
}
