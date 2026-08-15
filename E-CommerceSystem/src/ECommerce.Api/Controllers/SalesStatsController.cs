
using ECommerce.Application.Features.SalesStats.Dtos;
using ECommerce.Application.Features.SalesStats.Queries.GetSalesStats;
using ECommerce.Application.Features.SalesStats.Queries.GetProductSalesStats;
using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class SalesStatsController : ControllerBase
{
    private readonly ISender _sender;

    public SalesStatsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<SalesStatsDto>> GetSalesStats(
        [FromQuery] SalesStatsRange range = SalesStatsRange.Overall,
        [FromQuery] bool refresh = false)
    {
        var result = await _sender.Send(new GetSalesStatsQuery
        {
            Range = range,
            ForceRefresh = refresh
        });
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<ProductSalesStatsDto>> GetProductSalesStats(Guid productId)
    {
        var result = await _sender.Send(new GetProductSalesStatsQuery { ProductId = productId });
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
