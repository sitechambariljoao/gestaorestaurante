using GestaoRestaurante.Domain.Events;

namespace GestaoRestaurante.Application.Common.Events;

/// <summary>
/// Implementação base para notificações de domain events
/// </summary>
/// <typeparam name="TDomainEvent">Tipo do domain event</typeparam>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) 
    : IDomainEventNotification<TDomainEvent>
    where TDomainEvent : IDomainEvent;