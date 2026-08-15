using ECommerce.Application.Features.Payments.Commands.CreatePayment;
using ECommerce.Application.Features.Payments.Commands.ProcessPayment;
using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Application.Features.Payments.Queries.GetPaymentById;
using ECommerce.Application.Features.Payments.Queries.GetAllPayments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePaymentCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<PaymentDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAllPaymentsQuery());
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/process")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<PaymentDto>> Process(Guid id, [FromBody] ProcessPaymentCommand command)
    {
        command.PaymentId = id;
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetPaymentByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);
        return Ok(result.Value);
    }
}
