using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Payments.Queries.GetAllPayments;

public sealed record GetAllPaymentsQuery : IRequest<Result<List<PaymentDto>>>;

public sealed class GetAllPaymentsQueryHandler(IPaymentRepository payments)
    : IRequestHandler<GetAllPaymentsQuery, Result<List<PaymentDto>>>
{
    public async Task<Result<List<PaymentDto>>> Handle(
        GetAllPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await payments.GetAllAsync(cancellationToken);
        return Result.Success(result.Adapt<List<PaymentDto>>());
    }
}
