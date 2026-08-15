using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IInventoryTransactionRepository
{
    Task AddAsync(InventoryTransaction transaction, CancellationToken cancellationToken);
    Task<InventoryTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<InventoryTransaction>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
}
