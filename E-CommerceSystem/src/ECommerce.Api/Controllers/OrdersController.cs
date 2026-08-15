using ECommerce.Application.Features.Orders.Commands.CreateOrder;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Application.Features.Orders.Queries.GetOrderById;
using ECommerce.Application.Features.Orders.Queries.GetAllOrders;
using ECommerce.Application.Features.Orders.Queries.GetOrdersByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var result = await _sender.Send(new GetAllOrdersQuery());
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "customer,admin")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetOrderByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("customer/{customerId:guid}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<List<OrderDto>>> GetByCustomerId(Guid customerId)
    {
        var result = await _sender.Send(new GetOrdersByCustomerIdQuery { CustomerId = customerId });
        return Ok(result.Value);
    }
}
