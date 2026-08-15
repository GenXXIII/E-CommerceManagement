using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public sealed class Payment : AuditableEntity, IAggregateRoot
{
    private Payment()
    {
    }

    public Payment(
        Guid orderId,
        decimal amount,
        string paymentMethod)
    {
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public string PaymentMethod { get; private set; } = default!;

    public PaymentStatus Status { get; private set; }

    public Order Order { get; private set; } = default!;

    public Refund? Refund { get; private set; }

    public void MarkPaid()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be marked as paid.");

        Status = PaymentStatus.Paid;
        MarkUpdated();
    }

    public void MarkFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be marked as failed.");

        Status = PaymentStatus.Failed;
        MarkUpdated();
    }

    public void SetRefund(Refund refund)
    {
        Refund = refund;
        MarkUpdated();
    }
}
