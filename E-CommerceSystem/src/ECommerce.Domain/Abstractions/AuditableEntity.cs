namespace ECommerce.Domain.Abstractions;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}