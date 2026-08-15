using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using FluentValidation;

namespace ECommerce.Application.Features.CustomerProfiles.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
{
    public UpdateCustomerProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);
        });

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);
        });
    }
}
