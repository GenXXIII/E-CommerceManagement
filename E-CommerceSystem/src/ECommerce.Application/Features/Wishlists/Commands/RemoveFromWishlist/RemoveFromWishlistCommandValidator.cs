
using FluentValidation;

namespace ECommerce.Application.Features.Wishlists.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandValidator : AbstractValidator<RemoveFromWishlistCommand>
{
    public RemoveFromWishlistCommandValidator()
    {
        RuleFor(x => x.CustomerProfileId)
            .NotEmpty()
            .WithMessage("Customer profile ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
