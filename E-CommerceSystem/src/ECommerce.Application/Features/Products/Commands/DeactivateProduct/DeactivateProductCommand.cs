using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Products.Commands.DeactivateProduct;

public class DeactivateProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
