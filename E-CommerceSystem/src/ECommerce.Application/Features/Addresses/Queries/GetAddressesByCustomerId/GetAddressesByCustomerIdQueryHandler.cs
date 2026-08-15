using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQueryHandler : IRequestHandler<GetAddressesByCustomerIdQuery, Result<List<AddressDto>>>
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressesByCustomerIdQueryHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<Result<List<AddressDto>>> Handle(GetAddressesByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var addresses = await _addressRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
            var dtos = addresses.Adapt<List<AddressDto>>();
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<AddressDto>>(ex.Message);
        }
    }
}
