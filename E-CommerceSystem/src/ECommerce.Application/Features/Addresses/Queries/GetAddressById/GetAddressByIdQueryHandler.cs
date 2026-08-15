using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<AddressDto>>
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressByIdQueryHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<Result<AddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var address = await _addressRepository.GetByIdAsync(request.Id, cancellationToken);
            if (address == null)
                return Result.Failure<AddressDto>("Address not found.");

            var dto = address.Adapt<AddressDto>();
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AddressDto>(ex.Message);
        }
    }
}
