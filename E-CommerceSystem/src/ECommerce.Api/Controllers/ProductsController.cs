using ECommerce.Application.Features.Products.Commands.ActivateProduct;
using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Features.Products.Commands.DeactivateProduct;
using ECommerce.Application.Features.Products.Commands.DeleteProduct;
using ECommerce.Application.Features.Products.Commands.UpdateProduct;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using ECommerce.Application.Features.Products.Queries.SearchProducts;
using ECommerce.Application.Pagination;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Services;
using ECommerce.Domain.Enums;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ApplicationDbContext _dbContext;
    private readonly UploadedImageStorage _imageStorage;
    private readonly ICacheService _cacheService;

    public ProductsController(
        ISender sender,
        ApplicationDbContext dbContext,
        UploadedImageStorage imageStorage,
        ICacheService cacheService)
    {
        _sender = sender;
        _dbContext = dbContext;
        _imageStorage = imageStorage;
        _cacheService = cacheService;
    }

    [HttpPost("{id:guid}/image")]
    [Authorize(Roles = "admin")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<object>> UploadImage(Guid id, IFormFile image, CancellationToken cancellationToken)
    {
        var validationError = UploadedImageStorage.Validate(image, "Product");
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        var product = await _dbContext.Products
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
            return NotFound(new { message = "Product not found." });

        var imageUrl = await _imageStorage.SaveAsync(
            "productImage",
            id,
            image,
            cancellationToken);

        foreach (var existing in product.Images)
            _imageStorage.Delete("productImage", existing.ImageUrl, "products");

        _dbContext.ProductImages.RemoveRange(product.Images);
        await _dbContext.ProductImages.AddAsync(new ProductImage(id, imageUrl), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"product:{id}", cancellationToken);

        return Ok(new { imageUrl });
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value, includeHidden = true },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var imageUrls = await _dbContext.ProductImages
            .Where(image => image.ProductId == id)
            .Select(image => image.ImageUrl)
            .ToListAsync();
        var result = await _sender.Send(new DeleteProductCommand { Id = id });
        if (result.IsFailure)
            return BadRequest(result.Error);

        foreach (var imageUrl in imageUrls)
            _imageStorage.Delete("productImage", imageUrl, "products");
        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Activate(Guid id)
    {
        var result = await _sender.Send(new ActivateProductCommand { Id = id });
        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        var result = await _sender.Send(new DeactivateProductCommand { Id = id });
        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPatch("{id:guid}/fresh-tech-visibility")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> SetFreshTechVisibility(
        Guid id,
        [FromBody] VisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
            return NotFound(new { message = "Product not found." });

        product.SetFeatured(request.Visible);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"product:{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("products:*", cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(
        Guid id,
        [FromQuery] bool includeHidden = false)
    {
        var result = await _sender.Send(new GetProductByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);

        if (!includeHidden)
        {
            var isVisible = await _dbContext.Products
                .Where(product => product.Id == id)
                .Select(product =>
                    product.Status == ProductStatus.Active &&
                    product.Category.IsActive)
                .FirstOrDefaultAsync();
            if (!isVisible)
                return NotFound(new { message = "Product not found." });
        }

        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool includeHidden = false,
        [FromQuery] bool featuredOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SearchProductsQuery
        {
            Keyword = keyword,
            CategoryId = categoryId,
            IncludeHidden = includeHidden,
            FeaturedOnly = featuredOnly,
            Page = page,
            PageSize = pageSize
        };

        var result = await _sender.Send(query);
        return Ok(result.Value);
    }
}
