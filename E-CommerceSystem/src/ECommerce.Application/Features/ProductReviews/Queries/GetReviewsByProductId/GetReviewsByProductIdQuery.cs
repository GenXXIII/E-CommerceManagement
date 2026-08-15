using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Queries.GetReviewsByProductId;

public class GetReviewsByProductIdQuery : IRequest<Result<List<ProductReviewDto>>>
{
    public Guid ProductId { get; set; }
}
