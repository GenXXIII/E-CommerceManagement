
using FluentValidation;

namespace ECommerce.Application.Features.Wishlists.Commands.AddToWishlist;

public class AddToWishlistCommandValidator : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistCommandValidator()
    {
        RuleFor(x => x.CustomerProfileId)
            .NotEmpty()
            .WithMessage("Customer profile ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
