using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;

namespace ECommerce.Application.Services;

public sealed class OrderStockService : IOrderStockService
{
    private readonly IProductRepository _productRepository;
    private readonly IDistributedLockProvider _distributedLockProvider;

    public OrderStockService(
        IProductRepository productRepository,
        IDistributedLockProvider distributedLockProvider)
    {
        _productRepository = productRepository;
        _distributedLockProvider = distributedLockProvider;
    }

    public async Task<Result> ValidateAndDecreaseStockAsync(Domain.Entities.Order order, CancellationToken cancellationToken)
    {
        var lockTasks = new List<Task<IDistributedLock>>();
        foreach (var item in order.OrderItems)
        {
            var lockKey = $"product-stock-lock:{item.ProductId}";
            lockTasks.Add(_distributedLockProvider.AcquireLockAsync(
                resource: lockKey,
                expiryTime: TimeSpan.FromSeconds(30),
                waitTime: TimeSpan.FromSeconds(10),
                retryTime: TimeSpan.FromMilliseconds(500),
                cancellationToken: cancellationToken
            ));
        }

        var locks = await Task.WhenAll(lockTasks);
        try
        {
            if (locks.Any(l => !l.IsAcquired))
                return Result.Failure("Could not acquire lock to process order, please try again.");

            var productsById = new Dictionary<Guid, Domain.Entities.Product>();
            foreach (var item in order.OrderItems)
            {
                if (productsById.ContainsKey(item.ProductId))
                    continue;

                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                    return Result.Failure($"Product {item.ProductId} not found.");

                productsById[item.ProductId] = product;
            }

            foreach (var item in order.OrderItems)
            {
                var product = productsById[item.ProductId];
                if (product.Quantity < item.Quantity)
                    return Result.Failure("Insufficient stock.");
            }

            foreach (var item in order.OrderItems)
            {
                var product = productsById[item.ProductId];
                product.DecreaseStock(item.Quantity);
                _productRepository.Update(product);
            }

            return Result.Success();
        }
        finally
        {
            foreach (var redLock in locks)
            {
                await redLock.DisposeAsync();
            }
        }
    }
}

