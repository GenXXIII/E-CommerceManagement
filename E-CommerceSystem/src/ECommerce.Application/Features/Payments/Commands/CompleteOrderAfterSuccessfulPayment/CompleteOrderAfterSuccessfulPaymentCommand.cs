using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CompleteOrderAfterSuccessfulPayment;

public class CompleteOrderAfterSuccessfulPaymentCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
}

