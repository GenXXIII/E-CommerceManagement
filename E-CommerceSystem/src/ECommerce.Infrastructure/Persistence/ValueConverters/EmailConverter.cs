using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECommerce.Infrastructure.Persistence.ValueConverters;

public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() : base(
        email => email.Value,
        value => new Email(value))
    {
    }
}
