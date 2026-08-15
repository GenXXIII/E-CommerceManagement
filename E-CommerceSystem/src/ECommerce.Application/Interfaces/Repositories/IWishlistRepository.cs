
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IWishlistRepository
{
    Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken);
    void Update(Wishlist wishlist);
    Task<Wishlist?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
