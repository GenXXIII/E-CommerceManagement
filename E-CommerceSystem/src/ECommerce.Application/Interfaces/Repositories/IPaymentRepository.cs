using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken);
    void Update(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<List<Payment>> GetAllAsync(CancellationToken cancellationToken);
}
