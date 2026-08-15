using ECommerce.Application.Features.ProductReviews.Commands.CreateProductReview;
using ECommerce.Application.Features.ProductReviews.Commands.DeleteProductReview;
using ECommerce.Application.Features.ProductReviews.Commands.SetProductReviewVisibility;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Application.Features.ProductReviews.Queries.GetReviewsByProductId;
using ECommerce.Application.Features.ProductReviews.Queries.GetAllProductReviews;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ApplicationDbContext _dbContext;

    public ProductReviewsController(ISender sender, ApplicationDbContext dbContext)
    {
        _sender = sender;
        _dbContext = dbContext;
    }

    [HttpGet("storefront")]
    public async Task<ActionResult<List<StorefrontReviewDto>>> GetStorefrontReviews(
        [FromQuery] int limit = 6,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 12);
        var reviews = await _dbContext.ProductReviews
            .AsNoTracking()
            .Where(review =>
                review.Status == ReviewStatus.Visible &&
                review.Comment != null &&
                review.Comment != "" &&
                review.Product.Status == ProductStatus.Active &&
                review.Product.Category.IsActive)
            .OrderByDescending(review => review.CreatedAt)
            .Take(limit)
            .Select(review => new StorefrontReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product.Name,
                ProductImageUrl = review.Product.Images
                    .OrderBy(image => image.CreatedAt)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault(),
                CustomerName = review.CustomerProfile.FirstName,
                Rating = review.Rating,
                Comment = review.Comment!,
                CreatedAt = review.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(reviews);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<ProductReviewDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAllProductReviewsQuery());
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/visibility")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductReviewDto>> SetVisibility(
        Guid id,
        [FromBody] SetProductReviewVisibilityCommand command)
    {
        command.ReviewId = id;
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductReviewCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetByProductId), new { productId = command.ProductId }, result.Value);
    }

    [HttpDelete("{id:guid}/customer/{customerId:guid}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> Delete(Guid id, Guid customerId)
    {
        var result = await _sender.Send(new DeleteProductReviewCommand(id, customerId));
        if (result.IsFailure)
            return BadRequest(result.Error);
        return NoContent();
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<List<ProductReviewDto>>> GetByProductId(Guid productId)
    {
        var result = await _sender.Send(new GetReviewsByProductIdQuery { ProductId = productId });
        return Ok(result.Value);
    }

    [HttpGet("product/{productId:guid}/customer/{customerId:guid}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<ProductReviewDto?>> GetCustomerReview(
        Guid productId,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var review = await _dbContext.ProductReviews
            .AsNoTracking()
            .Where(item =>
                item.ProductId == productId &&
                item.CustomerProfileId == customerId)
            .Select(item => new ProductReviewDto
            {
                Id = item.Id,
                CustomerProfileId = item.CustomerProfileId,
                ProductId = item.ProductId,
                OrderId = item.OrderId,
                Rating = item.Rating,
                Comment = item.Comment,
                Status = item.Status,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Ok(review);
    }
}
