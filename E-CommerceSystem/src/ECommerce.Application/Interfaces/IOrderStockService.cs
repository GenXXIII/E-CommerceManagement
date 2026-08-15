using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IOrderStockService
{
    Task<Result> ValidateAndDecreaseStockAsync(Order order, CancellationToken cancellationToken);
}

