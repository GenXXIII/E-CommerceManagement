using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.InventoryTransactions.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.InventoryTransactions.Commands.CreateInventoryTransaction;

public class CreateInventoryTransactionCommand : IRequest<Result<Guid>>
{
    public Guid ProductId { get; set; }
    public Domain.Enums.InventoryTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
