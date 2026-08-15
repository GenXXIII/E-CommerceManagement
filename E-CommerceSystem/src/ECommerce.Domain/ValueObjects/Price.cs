using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Price
{
    public decimal Value { get; }

    public Price(decimal value)
    {
        if (value <= 0)
            throw new DomainException("Price must be greater than zero.");
        
        // Ensure only two decimal places for currency
        if (Math.Round(value, 2) != value)
            throw new DomainException("Price cannot have more than two decimal places.");

        Value = value;
    }

    public static implicit operator decimal(Price price) => price.Value;
    public static explicit operator Price(decimal value) => new(value);
}
