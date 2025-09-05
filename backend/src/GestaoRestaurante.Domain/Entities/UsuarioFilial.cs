using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class UsuarioFilial
{
    public Guid UsuarioId { get; set; }
    
    public Guid FilialId { get; set; }
    
    public DateTime DataVinculo { get; set; } = DateTime.Now;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime? DataDesvinculo { get; set; }
    
    [MaxLength(50)]
    public string? PerfilFilial { get; set; } // Perfil específico para esta filial
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Filial Filial { get; set; } = null!;
}