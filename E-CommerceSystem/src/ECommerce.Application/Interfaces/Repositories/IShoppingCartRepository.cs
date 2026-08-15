using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IShoppingCartRepository
{
    Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken);
    void Update(ShoppingCart cart);
    Task<int> DeleteAllItemsAsync(Guid customerId, CancellationToken cancellationToken);
    Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
