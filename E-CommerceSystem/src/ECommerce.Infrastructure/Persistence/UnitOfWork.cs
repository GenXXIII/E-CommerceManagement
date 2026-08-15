using ECommerce.Application.Interfaces;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IPublisher _publisher;

    public UnitOfWork(ApplicationDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get domain events from entities
        var domainEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // Clear domain events from entities
        foreach (var entry in _context.ChangeTracker.Entries<BaseEntity>())
        {
            entry.Entity.ClearDomainEvents();
        }

        // Save changes first
        await _context.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
