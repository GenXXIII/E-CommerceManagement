using System.Security.Claims;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public sealed class AuthController : ControllerBase
{
    private readonly ICustomerProfileRepository _customers;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(ICustomerProfileRepository customers, IUnitOfWork unitOfWork)
    {
        _customers = customers;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("session")]
    public async Task<ActionResult<AuthSessionResponse>> Session(CancellationToken cancellationToken)
    {
        var username = User.FindFirstValue("preferred_username") ?? User.Identity?.Name ?? "user";
        var displayName = User.FindFirstValue("name") ?? username;
        var isAdmin = User.IsInRole("admin");
        var isCustomer = User.IsInRole("customer");

        if (!isAdmin && !isCustomer)
            return Forbid();

        CustomerProfile? profile = null;
        if (isCustomer && !isAdmin)
        {
            var email = User.FindFirstValue("email");
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Your Keycloak account needs an email address." });

            profile = await _customers.GetByEmailAsync(email, cancellationToken);
            if (profile is null)
            {
                var firstName = User.FindFirstValue("given_name");
                var lastName = User.FindFirstValue("family_name");
                var nameParts = displayName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                firstName = string.IsNullOrWhiteSpace(firstName) ? nameParts.FirstOrDefault() ?? username : firstName;
                lastName = string.IsNullOrWhiteSpace(lastName) ? nameParts.Skip(1).FirstOrDefault() ?? "Customer" : lastName;

                profile = new CustomerProfile(firstName, lastName, new Email(email), string.Empty);
                await _customers.AddAsync(profile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return Ok(new AuthSessionResponse(
            username,
            displayName,
            isAdmin ? "admin" : "customer",
            profile?.Id));
    }
}

public sealed record AuthSessionResponse(
    string Username,
    string DisplayName,
    string Role,
    Guid? CustomerProfileId);
