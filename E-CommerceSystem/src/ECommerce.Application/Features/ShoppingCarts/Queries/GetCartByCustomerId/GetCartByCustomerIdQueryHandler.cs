using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ShoppingCarts.Queries.GetCartByCustomerId;

public class GetCartByCustomerIdQueryHandler : IRequestHandler<GetCartByCustomerIdQuery, Result<ShoppingCartDto>>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICacheService _cacheService;

    public GetCartByCustomerIdQueryHandler(IShoppingCartRepository cartRepository, ICacheService cacheService)
    {
        _cartRepository = cartRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<ShoppingCartDto>> Handle(GetCartByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"shoppingcart:{request.CustomerId}";
            var cachedCart = await _cacheService.GetAsync<ShoppingCartDto>(cacheKey, cancellationToken);
            if (cachedCart != null)
                return Result.Success(cachedCart);

            var cart = await _cartRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
            ShoppingCartDto dto;
            if (cart == null)
            {
                dto = new ShoppingCartDto
                {
                    CustomerProfileId = request.CustomerId,
                    Items = new(),
                    TotalAmount = 0
                };
            }
            else
            {
                dto = cart.Adapt<ShoppingCartDto>();
            }

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30), cancellationToken);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ShoppingCartDto>(ex.Message);
        }
    }
}
