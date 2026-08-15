using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Commands.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<ShoppingCartDto>>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public AddToCartCommandHandler(
        IShoppingCartRepository cartRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<ShoppingCartDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure<ShoppingCartDto>("Product not found.");

            var cart = await _cartRepository.GetByCustomerIdAsync(request.CustomerProfileId, cancellationToken);
            if (cart == null)
            {
                cart = new ShoppingCart(request.CustomerProfileId);
                await _cartRepository.AddAsync(cart, cancellationToken);
            }

            cart.AddItem(product, request.Quantity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var cacheKey = $"shoppingcart:{request.CustomerProfileId}";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            var dto = cart.Adapt<ShoppingCartDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ShoppingCartDto>(ex.Message);
        }
    }
}
