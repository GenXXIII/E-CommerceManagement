using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public sealed class Order : AuditableEntity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    {
    }

    public Order(
        Guid customerProfileId,
        Guid addressId,
        string? note)
    {
        CustomerProfileId = customerProfileId;
        AddressId = addressId;
        Note = note;
        Status = OrderStatus.PendingPayment;
        TotalAmount = 0;
    }

    public Guid CustomerProfileId { get; private set; }

    public Guid AddressId { get; private set; }

    public string? Note { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public CustomerProfile CustomerProfile { get; private set; } = default!;

    public Address Address { get; private set; } = default!;

    public Payment? Payment { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public void AddOrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        _orderItems.Add(new OrderItem(Id, productId, quantity, unitPrice));
        TotalAmount = _orderItems.Sum(x => x.TotalPrice);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.Pending)
            throw new DomainException("Only pending payment orders can be confirmed.");

        Status = OrderStatus.Confirmed;
        MarkUpdated();
    }

    public void MarkPaymentFailed()
    {
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.Pending)
            throw new DomainException("Only pending payment orders can be marked as payment failed.");

        Status = OrderStatus.PaymentFailed;
        MarkUpdated();
    }

    public void Pack()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be packed.");

        Status = OrderStatus.Packed;
        MarkUpdated();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Packed)
            throw new DomainException("Only packed orders can be shipped.");

        Status = OrderStatus.Shipped;
        MarkUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
        MarkUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new DomainException("Cannot cancel this order.");

        Status = OrderStatus.Cancelled;
        MarkUpdated();
    }

    public void SetPayment(Payment payment)
    {
        Payment = payment;
        MarkUpdated();
    }
}
