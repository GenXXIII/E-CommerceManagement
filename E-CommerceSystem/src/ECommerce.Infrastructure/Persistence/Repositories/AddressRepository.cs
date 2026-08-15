using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Address address, CancellationToken cancellationToken)
    {
        await _context.Addresses.AddAsync(address, cancellationToken);
    }

    public void Update(Address address)
    {
        _context.Addresses.Update(address);
    }

    public void Delete(Address address)
    {
        _context.Addresses.Remove(address);
    }

    public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Addresses.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Address>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .Where(x => x.CustomerProfileId == customerId)
            .ToListAsync(cancellationToken);
    }
}
