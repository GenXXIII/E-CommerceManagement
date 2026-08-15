using ECommerce.Application.Features.Refunds.Dtos;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Refunds.Queries.GetRefunds;

public sealed record GetAllRefundsQuery : IRequest<Result<List<RefundDto>>>;
public sealed record GetRefundsByCustomerIdQuery(Guid CustomerId) : IRequest<Result<List<RefundDto>>>;

public sealed class GetAllRefundsQueryHandler(IRefundRepository refunds)
    : IRequestHandler<GetAllRefundsQuery, Result<List<RefundDto>>>
{
    public async Task<Result<List<RefundDto>>> Handle(
        GetAllRefundsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await refunds.GetAllAsync(cancellationToken);
        return Result.Success(result.Adapt<List<RefundDto>>());
    }
}

public sealed class GetRefundsByCustomerIdQueryHandler(IRefundRepository refunds)
    : IRequestHandler<GetRefundsByCustomerIdQuery, Result<List<RefundDto>>>
{
    public async Task<Result<List<RefundDto>>> Handle(
        GetRefundsByCustomerIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await refunds.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return Result.Success(result.Adapt<List<RefundDto>>());
    }
}
