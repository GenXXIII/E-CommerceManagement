using ECommerce.Application.Features.Addresses.Commands.CreateAddress;
using ECommerce.Application.Features.Addresses.Commands.DeleteAddress;
using ECommerce.Application.Features.Addresses.Commands.UpdateAddress;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Application.Features.Addresses.Queries.GetAddressesByCustomerId;
using ECommerce.Application.Features.Addresses.Queries.GetAddressById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "customer")]
public class AddressesController : ControllerBase
{
    private readonly ISender _sender;

    public AddressesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateAddressCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAddressCommand command)
    {
        command.Id = id;
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _sender.Send(new DeleteAddressCommand { Id = id });
        if (result.IsFailure)
            return BadRequest(result.Error);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddressDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetAddressByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<List<AddressDto>>> GetByCustomerId(Guid customerId)
    {
        var result = await _sender.Send(new GetAddressesByCustomerIdQuery { CustomerId = customerId });
        return Ok(result.Value);
    }
}
