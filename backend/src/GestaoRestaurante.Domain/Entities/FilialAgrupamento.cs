using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

/// <summary>
/// Entidade de relacionamento N:N entre Filial e Agrupamento
/// Um agrupamento pode estar vinculado a múltiplas filiais
/// Uma filial pode ter múltiplos agrupamentos
/// </summary>
public class FilialAgrupamento
{
    [Required]
    public Guid FilialId { get; set; }
    
    [Required]
    public Guid AgrupamentoId { get; set; }
    
    public DateTime DataVinculo { get; set; }
    public bool Ativo { get; set; } = true;
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual Agrupamento Agrupamento { get; set; } = null!;
}