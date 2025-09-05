using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class AssinaturaEmpresa
{
    public Guid Id { get; set; }
    
    public Guid EmpresaId { get; set; }
    
    public Guid PlanoId { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataVencimento { get; set; }
    
    public bool Ativa { get; set; } = true;
    
    public bool RenovacaoAutomatica { get; set; } = false;
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataCancelamento { get; set; }
    
    // Relacionamentos
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual PlanoAssinatura Plano { get; set; } = null!;
}