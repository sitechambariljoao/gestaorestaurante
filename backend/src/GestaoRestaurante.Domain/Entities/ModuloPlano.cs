using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class ModuloPlano
{
    public Guid Id { get; set; }
    
    public Guid PlanoId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string NomeModulo { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    public bool Liberado { get; set; } = false;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual PlanoAssinatura Plano { get; set; } = null!;
}