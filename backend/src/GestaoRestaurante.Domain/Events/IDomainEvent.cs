namespace GestaoRestaurante.Domain.Events;

/// <summary>
/// Interface base para todos os eventos de domínio
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único do evento
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Data e hora em que o evento ocorreu
    /// </summary>
    DateTime OccurredAt { get; }
    
    /// <summary>
    /// Nome do tipo do evento
    /// </summary>
    string EventType { get; }
}