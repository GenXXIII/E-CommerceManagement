
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Wishlists.Queries.GetWishlistByCustomerId;

public class GetWishlistByCustomerIdQueryHandler : IRequestHandler<GetWishlistByCustomerIdQuery, Result<WishlistDto>>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ICacheService _cacheService;

    public GetWishlistByCustomerIdQueryHandler(IWishlistRepository wishlistRepository, ICacheService cacheService)
    {
        _wishlistRepository = wishlistRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<WishlistDto>> Handle(GetWishlistByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"wishlist:{request.CustomerId}";
            var cachedWishlist = await _cacheService.GetAsync<WishlistDto>(cacheKey, cancellationToken);
            if (cachedWishlist != null)
                return Result.Success(cachedWishlist);

            var wishlist = await _wishlistRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
            WishlistDto dto;
            if (wishlist == null)
            {
                dto = new WishlistDto
                {
                    CustomerProfileId = request.CustomerId,
                    Items = new()
                };
            }
            else
            {
                dto = wishlist.Adapt<WishlistDto>();
            }

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30), cancellationToken);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<WishlistDto>(ex.Message);
        }
    }
}
