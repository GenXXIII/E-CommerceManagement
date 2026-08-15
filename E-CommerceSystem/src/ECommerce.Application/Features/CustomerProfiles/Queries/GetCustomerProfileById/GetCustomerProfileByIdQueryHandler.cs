using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.CustomerProfiles.Dtos;
using ECommerce.Domain.Abstractions;
using MapsterMapper;
using MediatR;

namespace ECommerce.Application.Features.CustomerProfiles.Queries.GetCustomerProfileById;

public class GetCustomerProfileByIdQueryHandler : IRequestHandler<GetCustomerProfileByIdQuery, Result<CustomerProfileDto>>
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IMapper _mapper;

    public GetCustomerProfileByIdQueryHandler(
        ICustomerProfileRepository customerProfileRepository,
        IMapper mapper)
    {
        _customerProfileRepository = customerProfileRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerProfileDto>> Handle(
        GetCustomerProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customerProfile = await _customerProfileRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customerProfile == null)
            return Result.Failure<CustomerProfileDto>("Customer profile not found.");

        var dto = _mapper.Map<CustomerProfileDto>(customerProfile);
        return Result.Success(dto);
    }
}
