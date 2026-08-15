using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task AddAsync(Address address, CancellationToken cancellationToken);
    void Update(Address address);
    void Delete(Address address);
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Address>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
