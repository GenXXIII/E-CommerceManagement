
using ECommerce.Application.Features.Wishlists.Commands.AddToWishlist;
using ECommerce.Application.Features.Wishlists.Commands.ClearWishlist;
using ECommerce.Application.Features.Wishlists.Commands.RemoveFromWishlist;
using ECommerce.Application.Features.Wishlists.Dtos;
using ECommerce.Application.Features.Wishlists.Queries.GetWishlistByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "customer")]
public class WishlistsController : ControllerBase
{
    private readonly ISender _sender;

    public WishlistsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("add")]
    public async Task<ActionResult<WishlistDto>> AddToWishlist([FromBody] AddToWishlistCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("remove")]
    public async Task<ActionResult<WishlistDto>> RemoveFromWishlist([FromBody] RemoveFromWishlistCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("clear")]
    public async Task<ActionResult<WishlistDto>> ClearWishlist([FromBody] ClearWishlistCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<WishlistDto>> GetByCustomerId(Guid customerId)
    {
        var result = await _sender.Send(new GetWishlistByCustomerIdQuery { CustomerId = customerId });
        return Ok(result.Value);
    }
}
