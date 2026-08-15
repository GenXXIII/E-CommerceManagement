
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken)
    {
        await _context.Wishlists.AddAsync(wishlist, cancellationToken);
    }

    public void Update(Wishlist wishlist)
    {
        _context.Wishlists.Update(wishlist);
    }

    public async Task<Wishlist?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Wishlists
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.CustomerProfileId == customerId, cancellationToken);
    }
}
