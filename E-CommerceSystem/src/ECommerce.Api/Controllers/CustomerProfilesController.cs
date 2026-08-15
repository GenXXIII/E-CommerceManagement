
using ECommerce.Application.Features.CustomerProfiles.Commands.CreateCustomerProfile;
using ECommerce.Application.Features.CustomerProfiles.Commands.UpdateCustomerProfile;
using ECommerce.Application.Features.CustomerProfiles.Dtos;
using ECommerce.Application.Features.CustomerProfiles.Queries.GetCustomerProfileById;
using ECommerce.Application.Features.CustomerProfiles.Queries.GetAllCustomerProfiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerProfilesController : ControllerBase
{
    private readonly ISender _sender;

    public CustomerProfilesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<CustomerProfileDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAllCustomerProfilesQuery());
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCustomerProfileCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "customer,admin")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCustomerProfileCommand command)
    {
        command.Id = id;
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "customer,admin")]
    public async Task<ActionResult<CustomerProfileDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetCustomerProfileByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}
