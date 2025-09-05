using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class PlanoAssinatura
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }
    
    public int QuantidadeFiliais { get; set; } = 1;
    
    public int QuantidadeUsuarios { get; set; } = 1;
    
    public int DuracaoMeses { get; set; } = 1;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataUltimaAlteracao { get; set; }
    
    // Relacionamentos
    public virtual ICollection<ModuloPlano> Modulos { get; set; } = new List<ModuloPlano>();
    public virtual ICollection<AssinaturaEmpresa> Assinaturas { get; set; } = new List<AssinaturaEmpresa>();
}