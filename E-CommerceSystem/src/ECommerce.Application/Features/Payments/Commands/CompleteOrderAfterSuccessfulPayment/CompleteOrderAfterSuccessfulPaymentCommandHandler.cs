using ECommerce.Application.Features.Payments.Dtos;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Enums;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CompleteOrderAfterSuccessfulPayment;

public sealed class CompleteOrderAfterSuccessfulPaymentCommandHandler
    : IRequestHandler<CompleteOrderAfterSuccessfulPaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IOrderStockService _orderStockService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ISalesStatsService _salesStatsService;
    private readonly IOrderStatusNotifier _orderStatusNotifier;

    public CompleteOrderAfterSuccessfulPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IShoppingCartRepository shoppingCartRepository,
        IOrderStockService orderStockService,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ISalesStatsService salesStatsService,
        IOrderStatusNotifier orderStatusNotifier)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _orderStockService = orderStockService;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _salesStatsService = salesStatsService;
        _orderStatusNotifier = orderStatusNotifier;
    }

    public async Task<Result<PaymentDto>> Handle(CompleteOrderAfterSuccessfulPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
                return Result.Failure<PaymentDto>("Payment not found.");

            if (payment.Status != PaymentStatus.Pending)
                return Result.Failure<PaymentDto>("Only pending payments can be processed.");

            var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure<PaymentDto>("Order not found.");

            var stockResult = await _orderStockService.ValidateAndDecreaseStockAsync(order, cancellationToken);
            if (stockResult.IsFailure)
            {
                payment.MarkFailed();

                order.MarkPaymentFailed();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var failedDto = payment.Adapt<PaymentDto>();
                return Result.Success(failedDto);
            }

            payment.MarkPaid();

            order.Confirm();

            // Execute an explicit database DELETE. The ShoppingCart exposes a
            // read-only navigation, so collection change detection alone can
            // leave CartItems behind even though the payment is persisted.
            await _shoppingCartRepository.DeleteAllItemsAsync(
                order.CustomerProfileId,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // The cart query is cached for 30 minutes. Remove that snapshot
            // before returning so the frontend's refetch sees the empty cart.
            await _cacheService.RemoveAsync(
                $"shoppingcart:{order.CustomerProfileId}",
                cancellationToken);

            foreach (var item in order.OrderItems)
            {
                await _salesStatsService.RecordSuccessfulSaleAsync(
                    productId: item.ProductId,
                    quantitySold: item.Quantity,
                    totalRevenue: item.TotalPrice,
                    cancellationToken: cancellationToken);
            }

            await _orderStatusNotifier.NotifyOrderStatusChangedAsync(
                order.Id,
                nameof(OrderStatus.Confirmed),
                cancellationToken);

            var dto = payment.Adapt<PaymentDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<PaymentDto>(ex.Message);
        }
    }
}
