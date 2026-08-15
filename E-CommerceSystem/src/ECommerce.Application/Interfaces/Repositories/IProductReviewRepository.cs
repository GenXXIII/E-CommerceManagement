using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IProductReviewRepository
{
    Task AddAsync(ProductReview review, CancellationToken cancellationToken);
    void Update(ProductReview review);
    void Delete(ProductReview review);
    Task<ProductReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ProductReview>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<ProductReview?> GetByCustomerAndProductAsync(Guid customerId, Guid productId, CancellationToken cancellationToken);
    Task<List<ProductReview>> GetAllAsync(CancellationToken cancellationToken);
}
