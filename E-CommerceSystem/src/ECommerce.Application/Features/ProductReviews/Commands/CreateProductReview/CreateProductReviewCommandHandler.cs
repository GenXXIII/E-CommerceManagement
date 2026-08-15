using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.ProductReviews.Dtos;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, Result<Guid>>
{
    private readonly IProductReviewRepository _reviewRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerProfileRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductReviewCommandHandler(
        IProductReviewRepository reviewRepository,
        IOrderRepository orderRepository,
        ICustomerProfileRepository customerRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(
                request.CustomerProfileId,
                cancellationToken);
            if (customer is null)
                return Result.Failure<Guid>("Customer not found.");

            var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken);
            if (product is null)
                return Result.Failure<Guid>("Product not found.");

            if (request.OrderId.HasValue)
            {
                var order = await _orderRepository.GetByIdAsync(
                    request.OrderId.Value,
                    cancellationToken);
                if (order is null || order.CustomerProfileId != request.CustomerProfileId)
                    return Result.Failure<Guid>("The order does not belong to this customer.");

                if (!order.OrderItems.Any(item => item.ProductId == request.ProductId))
                    return Result.Failure<Guid>("This product is not part of the selected order.");

                if (order.Status is not (OrderStatus.Confirmed or OrderStatus.Packed or OrderStatus.Shipped or OrderStatus.Delivered))
                    return Result.Failure<Guid>("Only successfully paid orders can be marked as verified purchases.");
            }

            var existing = await _reviewRepository.GetByCustomerAndProductAsync(
                request.CustomerProfileId,
                request.ProductId,
                cancellationToken);
            if (existing is not null)
                return Result.Failure<Guid>("You have already reviewed this product.");

            var review = new ProductReview(
                request.CustomerProfileId,
                request.ProductId,
                request.OrderId,
                request.Rating,
                request.Comment);

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(review.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
