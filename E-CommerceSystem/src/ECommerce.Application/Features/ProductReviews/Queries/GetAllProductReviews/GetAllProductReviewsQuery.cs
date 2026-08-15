using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Queries.GetAllProductReviews;

public sealed record GetAllProductReviewsQuery : IRequest<Result<List<ProductReviewDto>>>;

public sealed class GetAllProductReviewsQueryHandler(IProductReviewRepository reviews)
    : IRequestHandler<GetAllProductReviewsQuery, Result<List<ProductReviewDto>>>
{
    public async Task<Result<List<ProductReviewDto>>> Handle(
        GetAllProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await reviews.GetAllAsync(cancellationToken);
        return Result.Success(result.Adapt<List<ProductReviewDto>>());
    }
}
