using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public sealed class Refund : AuditableEntity, IAggregateRoot
{
    private Refund()
    {
    }

    public Refund(
        Guid paymentId,
        decimal amount,
        string reason)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
        Status = RefundStatus.Pending;
    }

    public Guid PaymentId { get; private set; }

    public decimal Amount { get; private set; }

    public string Reason { get; private set; } = default!;

    public RefundStatus Status { get; private set; }

    public Payment Payment { get; private set; } = default!;

    public void Approve()
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException("Only pending refunds can be approved.");

        Status = RefundStatus.Approved;
        MarkUpdated();
    }

    public void Reject()
    {
        if (Status != RefundStatus.Pending)
            throw new DomainException("Only pending refunds can be rejected.");

        Status = RefundStatus.Rejected;
        MarkUpdated();
    }

    public void Complete()
    {
        if (Status != RefundStatus.Approved)
            throw new DomainException("Only approved refunds can be completed.");

        Status = RefundStatus.Completed;
        MarkUpdated();
    }
}
