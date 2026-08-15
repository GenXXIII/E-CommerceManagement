using ECommerce.Application.Features.ShoppingCarts.Commands.AddToCart;
using ECommerce.Application.Features.ShoppingCarts.Commands.RemoveFromCart;
using ECommerce.Application.Features.ShoppingCarts.Commands.UpdateCartItem;
using ECommerce.Application.Features.ShoppingCarts.Dtos;
using ECommerce.Application.Features.ShoppingCarts.Queries.GetCartByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "customer")]
public class ShoppingCartsController : ControllerBase
{
    private readonly ISender _sender;

    public ShoppingCartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("add")]
    public async Task<ActionResult<ShoppingCartDto>> AddToCart([FromBody] AddToCartCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("remove")]
    public async Task<ActionResult<ShoppingCartDto>> RemoveFromCart([FromBody] RemoveFromCartCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPut("update")]
    public async Task<ActionResult<ShoppingCartDto>> UpdateCartItem([FromBody] UpdateCartItemCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<ShoppingCartDto>> GetByCustomerId(Guid customerId)
    {
        var result = await _sender.Send(new GetCartByCustomerIdQuery { CustomerId = customerId });
        return Ok(result.Value);
    }
}
