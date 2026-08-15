using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Application.Features.CustomerProfiles.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.CustomerProfiles.Queries.GetCustomerProfileById;

public class GetCustomerProfileByIdQuery : IRequest<Result<CustomerProfileDto>>
{
    public Guid Id { get; set; }
}
