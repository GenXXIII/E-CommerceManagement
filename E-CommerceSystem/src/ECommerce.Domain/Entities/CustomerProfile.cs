using ECommerce.Domain.Abstractions;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public sealed class CustomerProfile : AuditableEntity, IAggregateRoot
{
    private readonly List<Address> _addresses = [];
    private readonly List<Order> _orders = [];
    private readonly List<ProductReview> _reviews = [];

    private CustomerProfile()
    {
    }

    public CustomerProfile(
        string firstName,
        string lastName,
        Email email,
        string phone)
    {
        UpdateName(firstName, lastName);
        Email = email;
        UpdatePhone(phone);

        IsActive = true;
    }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public Email Email { get; private set; } = default!;

    public string Phone { get; private set; } = default!;

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Address> Addresses => _addresses;

    public IReadOnlyCollection<Order> Orders => _orders;

    public IReadOnlyCollection<ProductReview> Reviews => _reviews;

    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        MarkUpdated();
    }

    public void UpdateEmail(Email email)
    {
        Email = email;

        MarkUpdated();
    }

    public void UpdatePhone(string phone)
    {
        Phone = phone.Trim();

        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}