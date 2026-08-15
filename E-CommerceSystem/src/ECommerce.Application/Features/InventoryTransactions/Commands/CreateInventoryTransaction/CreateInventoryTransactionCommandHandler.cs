using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.InventoryTransactions.Dtos;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.InventoryTransactions.Commands.CreateInventoryTransaction;

public class CreateInventoryTransactionCommandHandler : IRequestHandler<CreateInventoryTransactionCommand, Result<Guid>>
{
    private readonly IInventoryTransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryTransactionCommandHandler(IInventoryTransactionRepository transactionRepository, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInventoryTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = new InventoryTransaction(request.ProductId, request.Type, request.Quantity, request.Note);
            await _transactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(transaction.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
