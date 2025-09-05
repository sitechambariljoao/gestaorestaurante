using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class LogOperacao
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Operacao { get; set; } = string.Empty; // "CREATE", "UPDATE", "DELETE"
    
    [Required]
    [MaxLength(100)]
    public string Entidade { get; set; } = string.Empty;
    
    public Guid? EntidadeId { get; set; }
    
    public string? DadosAnteriores { get; set; } // JSON
    
    public string? DadosNovos { get; set; } // JSON
    
    public Guid UsuarioId { get; set; }
    
    public Guid EmpresaId { get; set; }
    
    public Guid? FilialId { get; set; }
    
    public DateTime DataOperacao { get; set; } = DateTime.Now;
    
    [MaxLength(50)]
    public string? EnderecoIp { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual Filial? Filial { get; set; }
}