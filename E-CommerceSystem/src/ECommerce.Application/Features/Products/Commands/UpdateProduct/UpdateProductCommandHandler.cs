

using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.ValueObjects;
using MediatR;

namespace ECommerce.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure("Product not found.");

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                product.UpdateName(request.Name);

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.UpdateDescription(request.Description);

            if (request.Price.HasValue)
                product.UpdatePrice(new Price(request.Price.Value));

            if (request.Quantity.HasValue)
                product.UpdateQuantity(request.Quantity.Value);

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync($"product:{request.Id}", cancellationToken);
            await _cacheService.RemoveByPatternAsync("products:*", cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
