using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.InventoryTransactions.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.InventoryTransactions.Queries.GetTransactionsByProductId;

public class GetTransactionsByProductIdQuery : IRequest<Result<List<InventoryTransactionDto>>>
{
    public Guid ProductId { get; set; }
}
