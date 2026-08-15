using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    private OrderItem()
    {
    }

    internal OrderItem(
        Guid orderId,
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => Quantity * UnitPrice;

    public Order Order { get; private set; } = default!;

    public Product Product { get; private set; } = default!;
}
