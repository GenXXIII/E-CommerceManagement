using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.InventoryTransactions.Commands.CreateInventoryTransaction;
using FluentValidation;

namespace ECommerce.Application.Features.InventoryTransactions.Commands.CreateInventoryTransaction;

public class CreateInventoryTransactionCommandValidator : AbstractValidator<CreateInventoryTransactionCommand>
{
    public CreateInventoryTransactionCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}
