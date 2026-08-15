using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Products.Commands.ActivateProduct;

public class ActivateProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
