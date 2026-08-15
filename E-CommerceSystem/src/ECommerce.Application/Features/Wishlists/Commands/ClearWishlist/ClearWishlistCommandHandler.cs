
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Wishlists.Commands.ClearWishlist;

public class ClearWishlistCommandHandler : IRequestHandler<ClearWishlistCommand, Result<WishlistDto>>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ClearWishlistCommandHandler(
        IWishlistRepository wishlistRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _wishlistRepository = wishlistRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<WishlistDto>> Handle(ClearWishlistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var wishlist = await _wishlistRepository.GetByCustomerIdAsync(request.CustomerProfileId, cancellationToken);
            if (wishlist == null)
                return Result.Failure<WishlistDto>("Wishlist not found.");

            wishlist.Clear();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await _cacheService.RemoveAsync($"wishlist:{request.CustomerProfileId}", cancellationToken);

            var dto = wishlist.Adapt<WishlistDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<WishlistDto>(ex.Message);
        }
    }
}
