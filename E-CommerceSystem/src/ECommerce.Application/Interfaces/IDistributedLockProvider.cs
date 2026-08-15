
namespace ECommerce.Application.Interfaces;

public interface IDistributedLockProvider
{
    Task<IDistributedLock> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, CancellationToken cancellationToken = default);
}

public interface IDistributedLock : IAsyncDisposable
{
    bool IsAcquired { get; }
}

