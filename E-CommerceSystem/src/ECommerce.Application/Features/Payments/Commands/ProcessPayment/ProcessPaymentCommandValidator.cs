using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}

