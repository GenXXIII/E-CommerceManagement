using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceiverName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Province)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.District)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Commune)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Street)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.CustomerProfile)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
