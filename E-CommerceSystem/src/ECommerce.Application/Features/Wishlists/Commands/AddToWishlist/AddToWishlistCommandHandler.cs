
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Wishlists.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Result<WishlistDto>>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public AddToWishlistCommandHandler(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<WishlistDto>> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure<WishlistDto>("Product not found.");

            var wishlist = await _wishlistRepository.GetByCustomerIdAsync(request.CustomerProfileId, cancellationToken);
            if (wishlist == null)
            {
                wishlist = new Wishlist(request.CustomerProfileId);
                await _wishlistRepository.AddAsync(wishlist, cancellationToken);
            }

            wishlist.AddItem(product);
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
