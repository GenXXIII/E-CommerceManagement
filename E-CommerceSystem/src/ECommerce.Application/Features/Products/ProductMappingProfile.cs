using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.Products;

public class ProductMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(destination => destination.ImageUrls,
                source => source.Images.OrderBy(image => image.CreatedAt).Select(image => image.ImageUrl));
    }
}
