using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Refunds.Commands.ApproveRefund;
using FluentValidation;

namespace ECommerce.Application.Features.Refunds.Commands.ApproveRefund;

public class ApproveRefundCommandValidator : AbstractValidator<ApproveRefundCommand>
{
    public ApproveRefundCommandValidator()
    {
        RuleFor(x => x.RefundId).NotEmpty();
    }
}
