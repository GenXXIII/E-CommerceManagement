using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.ProductReviews.Dtos;

public class ProductReviewDto
{
    public Guid Id { get; set; }
    public Guid CustomerProfileId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
