using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Update(Product product);
    void Delete(Product product);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Product>> SearchAsync(string? keyword, Guid? categoryId, CancellationToken cancellationToken);
    IQueryable<Product> GetQueryable();
}
