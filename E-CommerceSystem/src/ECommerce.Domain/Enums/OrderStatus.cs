namespace ECommerce.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    Confirmed,
    Packed,
    Shipped,
    Delivered,
    Cancelled,
    PendingPayment,
    PaymentFailed
}
