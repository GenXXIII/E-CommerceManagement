using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.Entities;

public sealed class Address : AuditableEntity
{
    private Address()
    {
    }

    public Address(
        Guid customerId,
        string receiverName,
        string phone,
        string province,
        string district,
        string commune,
        string street,
        bool isDefault)
    {
        CustomerProfileId = customerId;
        ReceiverName = receiverName;
        Phone = phone;
        Province = province;
        District = district;
        Commune = commune;
        Street = street;
        IsDefault = isDefault;
    }

    public Guid CustomerProfileId { get; private set; }

    public string ReceiverName { get; private set; } = default!;

    public string Phone { get; private set; } = default!;

    public string Province { get; private set; } = default!;

    public string District { get; private set; } = default!;

    public string Commune { get; private set; } = default!;

    public string Street { get; private set; } = default!;

    public bool IsDefault { get; private set; }

    public CustomerProfile CustomerProfile { get; private set; } = default!;

    public void SetDefault()
    {
        IsDefault = true;
        MarkUpdated();
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        MarkUpdated();
    }
}