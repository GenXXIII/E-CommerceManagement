using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdQuery : IRequest<Result<AddressDto>>
{
    public Guid Id { get; set; }
}
