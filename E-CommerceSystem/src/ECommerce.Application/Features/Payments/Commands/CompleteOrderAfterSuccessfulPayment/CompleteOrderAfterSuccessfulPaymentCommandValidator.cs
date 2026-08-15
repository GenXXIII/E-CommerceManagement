using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.CompleteOrderAfterSuccessfulPayment;

public sealed class CompleteOrderAfterSuccessfulPaymentCommandValidator : AbstractValidator<CompleteOrderAfterSuccessfulPaymentCommand>
{
    public CompleteOrderAfterSuccessfulPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}

