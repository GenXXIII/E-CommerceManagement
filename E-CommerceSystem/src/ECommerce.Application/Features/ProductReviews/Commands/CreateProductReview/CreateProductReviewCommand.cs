using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewCommand : IRequest<Result<Guid>>
{
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
