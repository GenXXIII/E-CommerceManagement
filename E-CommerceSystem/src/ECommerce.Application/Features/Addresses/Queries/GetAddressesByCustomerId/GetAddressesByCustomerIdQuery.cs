using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQuery : IRequest<Result<List<AddressDto>>>
{
    public Guid CustomerId { get; set; }
}
