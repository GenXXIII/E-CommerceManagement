using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface ICustomerProfileRepository
{
    Task AddAsync(CustomerProfile customerProfile, CancellationToken cancellationToken);
    void Update(CustomerProfile customerProfile);
    Task<CustomerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CustomerProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<List<CustomerProfile>> GetAllAsync(CancellationToken cancellationToken);
}
