using MediatR;
using GestaoRestaurante.Domain.Events;

namespace GestaoRestaurante.Application.Common.Events;

/// <summary>
/// Serviço para publicar domain events via MediatR
/// </summary>
public interface IDomainEventPublisher
{
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;
        
    Task PublishAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

public sealed class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IPublisher _publisher;

    public DomainEventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var notification = new DomainEventNotification<TDomainEvent>(domainEvent);
        await _publisher.Publish(notification, cancellationToken);
    }

    public async Task PublishAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            // Usa reflexão para chamar PublishAsync com o tipo correto
            var method = typeof(IDomainEventPublisher)
                .GetMethod(nameof(PublishAsync))!
                .MakeGenericMethod(domainEvent.GetType());
                
            var task = (Task)method.Invoke(this, [domainEvent, cancellationToken])!;
            await task;
        }
    }
}