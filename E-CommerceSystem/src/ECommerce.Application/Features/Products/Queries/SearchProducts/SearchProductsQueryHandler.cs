using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Application.Pagination;
using ECommerce.Domain.Abstractions;
using MapsterMapper;
using MediatR;

namespace ECommerce.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _productRepository.GetQueryable();

        if (!request.IncludeHidden)
        {
            query = query.Where(x =>
                x.Status == ECommerce.Domain.Enums.ProductStatus.Active &&
                x.Category.IsActive);
        }

        if (request.FeaturedOnly)
            query = query.Where(x => x.IsFeatured);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.Description.Contains(keyword));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        // Get paginated products first, then map to DTOs
        var paginatedProducts = await PaginatedList<ECommerce.Domain.Entities.Product>.CreateAsync(
            query,
            request.Page,
            request.PageSize,
            cancellationToken);

        var productDtos = _mapper.Map<List<ProductDto>>(paginatedProducts.Items);

        var paginatedDtoList = new PaginatedList<ProductDto>(
            productDtos,
            paginatedProducts.TotalCount,
            request.Page,
            request.PageSize);

        return Result.Success(paginatedDtoList);
    }
}
