using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.CustomerProfiles.Commands.CreateCustomerProfile;

public class CreateCustomerProfileCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
