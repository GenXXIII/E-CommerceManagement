using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Refund refund, CancellationToken cancellationToken)
    {
        await _context.Refunds.AddAsync(refund, cancellationToken);
    }

    public void Update(Refund refund)
    {
        _context.Refunds.Update(refund);
    }

    public async Task<Refund?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Refunds.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Refund>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Refunds
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Refund>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Refunds
            .AsNoTracking()
            .Where(x => x.Payment.Order.CustomerProfileId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
