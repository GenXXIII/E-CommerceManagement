using ECommerce.Domain.Abstractions;
using System.Text.RegularExpressions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email is required.");

        var trimmedEmail = value.Trim();
        
        if (!EmailRegex.IsMatch(trimmedEmail))
            throw new DomainException("Invalid email format.");

        Value = trimmedEmail;
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);
}
