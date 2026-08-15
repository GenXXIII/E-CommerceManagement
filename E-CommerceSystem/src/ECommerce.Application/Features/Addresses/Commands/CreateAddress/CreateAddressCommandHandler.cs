using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result<Guid>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAddressCommandHandler(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var address = new Address(
                request.CustomerProfileId,
                request.ReceiverName,
                request.Phone,
                request.Province,
                request.District,
                request.Commune,
                request.Street,
                request.IsDefault);

            await _addressRepository.AddAsync(address, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(address.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
