using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
