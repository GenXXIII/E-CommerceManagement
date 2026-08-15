using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
