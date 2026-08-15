using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Queries.GetReviewsByProductId;

public class GetReviewsByProductIdQueryHandler : IRequestHandler<GetReviewsByProductIdQuery, Result<List<ProductReviewDto>>>
{
    private readonly IProductReviewRepository _reviewRepository;

    public GetReviewsByProductIdQueryHandler(IProductReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<List<ProductReviewDto>>> Handle(GetReviewsByProductIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            var dtos = reviews.Adapt<List<ProductReviewDto>>();
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<ProductReviewDto>>(ex.Message);
        }
    }
}
