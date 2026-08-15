using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class CustomerProfileRepository : ICustomerProfileRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CustomerProfile customer, CancellationToken cancellationToken)
    {
        await _context.CustomerProfiles.AddAsync(customer, cancellationToken);
    }

    public void Update(CustomerProfile customer)
    {
        _context.CustomerProfiles.Update(customer);
    }

    public async Task<CustomerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.CustomerProfiles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<CustomerProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = new Email(email.Trim());
        return await _context.CustomerProfiles
            .FirstOrDefaultAsync(
                customer => customer.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task<List<CustomerProfile>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.CustomerProfiles
            .AsNoTracking()
            .OrderByDescending(customer => customer.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
