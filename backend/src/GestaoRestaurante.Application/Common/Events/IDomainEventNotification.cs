using MediatR;
using GestaoRestaurante.Domain.Events;

namespace GestaoRestaurante.Application.Common.Events;

/// <summary>
/// Interface para notificações de domain events
/// </summary>
/// <typeparam name="TDomainEvent">Tipo do domain event</typeparam>
public interface IDomainEventNotification<out TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    TDomainEvent DomainEvent { get; }
}