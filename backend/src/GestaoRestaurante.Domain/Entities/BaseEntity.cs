using GestaoRestaurante.Domain.Events;

namespace GestaoRestaurante.Domain.Entities;

public abstract class BaseEntity : IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; set; }
    public bool Ativa { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimaAlteracao { get; set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AtualizarTimestamp()
    {
        DataUltimaAlteracao = DateTime.UtcNow;
    }

    public virtual void Ativar()
    {
        Ativa = true;
        AtualizarTimestamp();
    }

    public virtual void Inativar()
    {
        Ativa = false;
        AtualizarTimestamp();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}