using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Commands.SetProductReviewVisibility;

public sealed class SetProductReviewVisibilityCommand : IRequest<Result<ProductReviewDto>>
{
    public Guid ReviewId { get; set; }
    public bool Visible { get; set; }
}

public sealed class SetProductReviewVisibilityCommandHandler(
    IProductReviewRepository reviews,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetProductReviewVisibilityCommand, Result<ProductReviewDto>>
{
    public async Task<Result<ProductReviewDto>> Handle(
        SetProductReviewVisibilityCommand request,
        CancellationToken cancellationToken)
    {
        var review = await reviews.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result.Failure<ProductReviewDto>("Review not found.");

        if (request.Visible) review.Show();
        else review.Hide();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(review.Adapt<ProductReviewDto>());
    }
}
