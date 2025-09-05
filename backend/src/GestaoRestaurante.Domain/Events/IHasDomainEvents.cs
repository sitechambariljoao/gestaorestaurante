namespace GestaoRestaurante.Domain.Events;

/// <summary>
/// Interface para entidades que podem gerar eventos de domínio
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Lista de eventos de domínio não processados
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Adiciona um evento de domínio à lista
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Remove um evento de domínio da lista
    /// </summary>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Limpa todos os eventos de domínio
    /// </summary>
    void ClearDomainEvents();
}