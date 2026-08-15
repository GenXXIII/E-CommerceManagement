
namespace ECommerce.Application.Interfaces;

public interface IOrderStatusNotifier
{
    Task NotifyOrderStatusChangedAsync(Guid orderId, string status, CancellationToken cancellationToken = default);
}

