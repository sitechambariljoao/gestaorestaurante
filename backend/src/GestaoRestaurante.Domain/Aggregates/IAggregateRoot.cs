namespace GestaoRestaurante.Domain.Aggregates;

/// <summary>
/// Interface que marca uma entidade como raiz de agregado
/// Agregados são clusters de objetos de domínio que podem ser tratados como uma única unidade
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Identificador único da raiz do agregado
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Versão do agregado para controle de concorrência otimista
    /// </summary>
    long Version { get; }
    
    /// <summary>
    /// Valida todos os invariantes do agregado
    /// </summary>
    void ValidateInvariants();
}