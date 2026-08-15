using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken)
    {
        await _context.ShoppingCarts.AddAsync(cart, cancellationToken);
    }

    public void Update(ShoppingCart cart)
    {
        _context.ShoppingCarts.Update(cart);
    }

    public Task<int> DeleteAllItemsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return _context.CartItems
            .Where(item => item.ShoppingCart.CustomerProfileId == customerId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerProfileId == customerId, cancellationToken);
    }
}
