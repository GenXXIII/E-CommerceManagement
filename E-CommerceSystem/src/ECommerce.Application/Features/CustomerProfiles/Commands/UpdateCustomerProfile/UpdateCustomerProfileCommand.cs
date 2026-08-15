using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.CustomerProfiles.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
