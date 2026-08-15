using ECommerce.Application.Features.InventoryTransactions.Commands.CreateInventoryTransaction;
using ECommerce.Application.Features.InventoryTransactions.Dtos;
using ECommerce.Application.Features.InventoryTransactions.Queries.GetTransactionsByProductId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class InventoryTransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public InventoryTransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateInventoryTransactionCommand command)
    {
        var result = await _sender.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetByProductId), new { productId = command.ProductId }, result.Value);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<List<InventoryTransactionDto>>> GetByProductId(Guid productId)
    {
        var result = await _sender.Send(new GetTransactionsByProductIdQuery { ProductId = productId });
        return Ok(result.Value);
    }
}
