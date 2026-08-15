using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Infrastructure.Persistence;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();

        options.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=ECommerce;Trusted_Connection=True;TrustServerCertificate=True");

        return new ApplicationDbContext(options.Options);
    }
}