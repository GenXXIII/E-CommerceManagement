using ECommerce.Application.Features.Refunds.Commands.ApproveRefund;
using ECommerce.Application.Features.Refunds.Commands.CreateRefund;
using ECommerce.Application.Features.Refunds.Dtos;
using ECommerce.Application.Features.Refunds.Queries.GetRefunds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundsController : ControllerBase
{
    private readonly ISender _sender;

    public RefundsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<RefundDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAllRefundsQuery());
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("customer/{customerId:guid}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<List<RefundDto>>> GetByCustomerId(Guid customerId)
    {
        var result = await _sender.Send(new GetRefundsByCustomerIdQuery(customerId));
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateRefundCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(Approve), new { id = result.Value }, result.Value);
    }

    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<RefundDto>> Approve(Guid id)
    {
        var result = await _sender.Send(new ApproveRefundCommand { RefundId = id });
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }
}
