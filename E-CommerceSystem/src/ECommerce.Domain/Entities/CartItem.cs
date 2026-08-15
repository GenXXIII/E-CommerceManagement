using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    private CartItem()
    {
    }

    internal CartItem(
        Guid shoppingCartId,
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid ShoppingCartId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => Quantity * UnitPrice;

    public ShoppingCart ShoppingCart { get; private set; } = default!;

    public Product Product { get; private set; } = default!;

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Invalid quantity.");

        Quantity += quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
    }
}