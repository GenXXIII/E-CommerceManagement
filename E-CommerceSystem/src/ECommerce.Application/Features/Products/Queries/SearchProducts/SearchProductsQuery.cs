using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Application.Pagination;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQuery : IRequest<Result<PaginatedList<ProductDto>>>
{
    public string? Keyword { get; set; }
    public Guid? CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeHidden { get; set; }
    public bool FeaturedOnly { get; set; }
}
