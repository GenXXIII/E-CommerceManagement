using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure<Guid>("Order not found.");

            var payment = new Payment(order.Id, request.Amount, request.PaymentMethod);
            await _paymentRepository.AddAsync(payment, cancellationToken);
            order.SetPayment(payment);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(payment.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
