using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Refunds.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Refunds.Commands.ApproveRefund;

public class ApproveRefundCommandHandler : IRequestHandler<ApproveRefundCommand, Result<RefundDto>>
{
    private readonly IRefundRepository _refundRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveRefundCommandHandler(IRefundRepository refundRepository, IUnitOfWork unitOfWork)
    {
        _refundRepository = refundRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefundDto>> Handle(ApproveRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var refund = await _refundRepository.GetByIdAsync(request.RefundId, cancellationToken);
            if (refund == null)
                return Result.Failure<RefundDto>("Refund not found.");

            refund.Approve();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var dto = refund.Adapt<RefundDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<RefundDto>(ex.Message);
        }
    }
}
