using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    void Update(Order order);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);
}
