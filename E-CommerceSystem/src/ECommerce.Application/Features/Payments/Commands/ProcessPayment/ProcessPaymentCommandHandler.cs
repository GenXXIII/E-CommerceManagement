using ECommerce.Application.Features.Payments.Commands.CompleteOrderAfterSuccessfulPayment;
using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ISender sender)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _sender = sender;
    }

    public async Task<Result<PaymentDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.SimulateSuccess)
            {
                return await _sender.Send(
                    new CompleteOrderAfterSuccessfulPaymentCommand { PaymentId = request.PaymentId },
                    cancellationToken);
            }

            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
                return Result.Failure<PaymentDto>("Payment not found.");

            if (payment.Status != PaymentStatus.Pending)
                return Result.Failure<PaymentDto>("Only pending payments can be processed.");

            var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure<PaymentDto>("Order not found.");

            payment.MarkFailed();

            order.MarkPaymentFailed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = payment.Adapt<PaymentDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<PaymentDto>(ex.Message);
        }
    }
}
