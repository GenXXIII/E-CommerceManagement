using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IRefundRepository
{
    Task AddAsync(Refund refund, CancellationToken cancellationToken);
    void Update(Refund refund);
    Task<Refund?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Refund>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Refund>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
