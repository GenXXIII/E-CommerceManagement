using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.ProductReviews;

public class ProductReviewMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductReview, ProductReviewDto>();
    }
}
