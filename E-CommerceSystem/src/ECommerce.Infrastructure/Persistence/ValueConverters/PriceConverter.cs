using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECommerce.Infrastructure.Persistence.ValueConverters;

public class PriceConverter : ValueConverter<Price, decimal>
{
    public PriceConverter() : base(
        price => price.Value,
        value => new Price(value))
    {
    }
}
