using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Payments.Queries.GetPaymentById;

public class GetPaymentByIdQuery : IRequest<Result<PaymentDto>>
{
    public Guid Id { get; set; }
}
