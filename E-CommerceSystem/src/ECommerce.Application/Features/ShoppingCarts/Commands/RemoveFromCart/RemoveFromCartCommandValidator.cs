using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ShoppingCarts.Commands.RemoveFromCart;
using FluentValidation;

namespace ECommerce.Application.Features.ShoppingCarts.Commands.RemoveFromCart;

public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.CustomerProfileId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}
