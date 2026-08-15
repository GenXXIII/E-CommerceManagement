
using FluentValidation;

namespace ECommerce.Application.Features.Wishlists.Commands.ClearWishlist;

public class ClearWishlistCommandValidator : AbstractValidator<ClearWishlistCommand>
{
    public ClearWishlistCommandValidator()
    {
        RuleFor(x => x.CustomerProfileId)
            .NotEmpty()
            .WithMessage("Customer profile ID is required.");
    }
}
