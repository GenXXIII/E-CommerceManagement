using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.InventoryTransactions.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.InventoryTransactions.Queries.GetTransactionsByProductId;

public class GetTransactionsByProductIdQueryHandler : IRequestHandler<GetTransactionsByProductIdQuery, Result<List<InventoryTransactionDto>>>
{
    private readonly IInventoryTransactionRepository _transactionRepository;

    public GetTransactionsByProductIdQueryHandler(IInventoryTransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<List<InventoryTransactionDto>>> Handle(GetTransactionsByProductIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _transactionRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            var dtos = transactions.Adapt<List<InventoryTransactionDto>>();
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<InventoryTransactionDto>>(ex.Message);
        }
    }
}
