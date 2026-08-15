using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Commands.DeleteProductReview;

public sealed record DeleteProductReviewCommand(Guid ReviewId, Guid CustomerProfileId) : IRequest<Result>;

public sealed class DeleteProductReviewCommandHandler(
    IProductReviewRepository reviews,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductReviewCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProductReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await reviews.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result.Failure("Review not found.");

        if (review.CustomerProfileId != request.CustomerProfileId)
            return Result.Failure("You can only remove your own review.");

        reviews.Delete(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
