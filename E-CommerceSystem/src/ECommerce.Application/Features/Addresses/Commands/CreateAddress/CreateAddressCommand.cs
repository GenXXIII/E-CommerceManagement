using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommand : IRequest<Result<Guid>>
{
    public Guid CustomerProfileId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Commune { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
