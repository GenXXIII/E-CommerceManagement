using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
    public bool SimulateSuccess { get; set; }
}

