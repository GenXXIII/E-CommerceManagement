using ECommerce.Application.Features.Categories.Commands.CreateCategory;
using ECommerce.Application.Features.Categories.Commands.DeleteCategory;
using ECommerce.Application.Features.Categories.Commands.UpdateCategory;
using ECommerce.Application.Features.Categories.Dtos;
using ECommerce.Application.Features.Categories.Queries.GetAllCategories;
using ECommerce.Application.Features.Categories.Queries.GetCategoryById;
using ECommerce.Api.Services;
using ECommerce.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ApplicationDbContext _dbContext;
    private readonly UploadedImageStorage _imageStorage;

    public CategoriesController(
        ISender sender,
        ApplicationDbContext dbContext,
        UploadedImageStorage imageStorage)
    {
        _sender = sender;
        _dbContext = dbContext;
        _imageStorage = imageStorage;
    }

    [HttpPost("{id:guid}/image")]
    [Authorize(Roles = "admin")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<object>> UploadImage(
        Guid id,
        IFormFile image,
        CancellationToken cancellationToken)
    {
        var validationError = UploadedImageStorage.Validate(image, "Category");
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
            return NotFound(new { message = "Category not found." });

        var previousImageUrl = category.ImageUrl;
        var imageUrl = await _imageStorage.SaveAsync(
            "CategoryImage",
            id,
            image,
            cancellationToken);

        category.SetImage(imageUrl);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _imageStorage.Delete("CategoryImage", previousImageUrl);

        return Ok(new { imageUrl });
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryCommand command)
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
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
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
        var imageUrl = await _dbContext.Categories
            .Where(category => category.Id == id)
            .Select(category => category.ImageUrl)
            .FirstOrDefaultAsync();
        var result = await _sender.Send(new DeleteCategoryCommand { Id = id });
        if (result.IsFailure)
            return BadRequest(result.Error);

        _imageStorage.Delete("CategoryImage", imageUrl);
        return NoContent();
    }

    [HttpPatch("{id:guid}/visibility")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> SetVisibility(
        Guid id,
        [FromBody] VisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
            return NotFound(new { message = "Category not found." });

        if (request.Visible) category.Activate();
        else category.Deactivate();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        Guid id,
        [FromQuery] bool includeHidden = false)
    {
        var result = await _sender.Send(new GetCategoryByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);

        if (!includeHidden && !result.Value.IsActive)
            return NotFound(new { message = "Category not found." });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        [FromQuery] bool includeHidden = false)
    {
        var result = await _sender.Send(new GetAllCategoriesQuery
        {
            IncludeHidden = includeHidden
        });
        return Ok(result.Value);
    }
}

public sealed record VisibilityRequest(bool Visible);
