using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(InventoryTransaction transaction, CancellationToken cancellationToken)
    {
        await _context.InventoryTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task<InventoryTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.InventoryTransactions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<InventoryTransaction>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.InventoryTransactions
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
}
