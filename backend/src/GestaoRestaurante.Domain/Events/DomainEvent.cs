namespace GestaoRestaurante.Domain.Events;

/// <summary>
/// Classe base abstrata para eventos de domínio
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = GetType().Name;
    }
}