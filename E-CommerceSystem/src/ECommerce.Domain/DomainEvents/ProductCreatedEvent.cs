using ECommerce.Domain.Abstractions;

namespace ECommerce.Domain.DomainEvents;

public record ProductCreatedEvent(Guid ProductId, string ProductName) : IDomainEvent;
