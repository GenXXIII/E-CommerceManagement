using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Refunds.Dtos;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Refunds.Commands.CreateRefund;

public class CreateRefundCommandHandler : IRequestHandler<CreateRefundCommand, Result<Guid>>
{
    private readonly IRefundRepository _refundRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRefundCommandHandler(
        IRefundRepository refundRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _refundRepository = refundRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
                return Result.Failure<Guid>("Payment not found.");

            if (payment.Status != PaymentStatus.Paid)
                return Result.Failure<Guid>("Only paid payments can be refunded.");

            if (payment.Refund is not null)
                return Result.Failure<Guid>("A refund already exists for this payment.");

            if (request.Amount > payment.Amount)
                return Result.Failure<Guid>("Refund amount cannot exceed the paid amount.");

            var refund = new Refund(payment.Id, request.Amount, request.Reason);
            await _refundRepository.AddAsync(refund, cancellationToken);
            payment.SetRefund(refund);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(refund.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
