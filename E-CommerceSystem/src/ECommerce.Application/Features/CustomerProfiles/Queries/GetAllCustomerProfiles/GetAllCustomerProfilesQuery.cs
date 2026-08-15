using ECommerce.Application.Features.CustomerProfiles.Dtos;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Abstractions;
using Mapster;
using MediatR;

namespace ECommerce.Application.Features.CustomerProfiles.Queries.GetAllCustomerProfiles;

public sealed record GetAllCustomerProfilesQuery : IRequest<Result<List<CustomerProfileDto>>>;

public sealed class GetAllCustomerProfilesQueryHandler(ICustomerProfileRepository customers)
    : IRequestHandler<GetAllCustomerProfilesQuery, Result<List<CustomerProfileDto>>>
{
    public async Task<Result<List<CustomerProfileDto>>> Handle(
        GetAllCustomerProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await customers.GetAllAsync(cancellationToken);
        return Result.Success(result.Adapt<List<CustomerProfileDto>>());
    }
}
