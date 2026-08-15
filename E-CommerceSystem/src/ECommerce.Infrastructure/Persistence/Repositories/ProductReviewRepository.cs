using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class ProductReviewRepository : IProductReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ProductReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProductReview review, CancellationToken cancellationToken)
    {
        await _context.ProductReviews.AddAsync(review, cancellationToken);
    }

    public void Update(ProductReview review)
    {
        _context.ProductReviews.Update(review);
    }

    public void Delete(ProductReview review)
    {
        _context.ProductReviews.Remove(review);
    }

    public async Task<ProductReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.ProductReviews.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<ProductReview>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.ProductReviews
            .Where(x => x.ProductId == productId && x.Status == ReviewStatus.Visible)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductReview?> GetByCustomerAndProductAsync(Guid customerId, Guid productId, CancellationToken cancellationToken)
    {
        return await _context.ProductReviews
            .FirstOrDefaultAsync(x => x.CustomerProfileId == customerId && x.ProductId == productId, cancellationToken);
    }

    public async Task<List<ProductReview>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.ProductReviews
            .AsNoTracking()
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
