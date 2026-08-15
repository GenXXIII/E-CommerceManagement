using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<ShoppingCartDto>>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateCartItemCommandHandler(IShoppingCartRepository cartRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<ShoppingCartDto>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(request.CustomerProfileId, cancellationToken);
            if (cart == null)
                return Result.Failure<ShoppingCartDto>("Cart not found.");

            cart.UpdateItemQuantity(request.ProductId, request.Quantity);
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
