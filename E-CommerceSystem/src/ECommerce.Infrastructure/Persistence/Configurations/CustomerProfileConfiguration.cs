using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion<EmailConverter>();

        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.CustomerProfile)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.CustomerProfile)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Reviews)
            .WithOne(x => x.CustomerProfile)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
