using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using FluentValidation;

namespace ECommerce.Application.Features.CustomerProfiles.Commands.CreateCustomerProfile;

public class CreateCustomerProfileCommandValidator : AbstractValidator<CreateCustomerProfileCommand>
{
    public CreateCustomerProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
    }
}
