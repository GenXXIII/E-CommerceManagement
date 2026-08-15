using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public sealed class InventoryTransaction : AuditableEntity, IAggregateRoot
{
    private InventoryTransaction()
    {
    }

    public InventoryTransaction(
        Guid productId,
        InventoryTransactionType type,
        int quantity,
        string? note)
    {
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        Note = note;
    }

    public Guid ProductId { get; private set; }

    public InventoryTransactionType Type { get; private set; }

    public int Quantity { get; private set; }

    public string? Note { get; private set; }

    public Product Product { get; private set; } = default!;
}
