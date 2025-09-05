using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class MovimentacaoEstoque
{
    public Guid Id { get; set; }
    
    public Guid? ProdutoId { get; set; }
    
    public Guid? IngredienteId { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    public Guid FilialId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TipoMovimentacao { get; set; } = string.Empty; // "ENTRADA", "SAIDA", "AJUSTE"
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantidade { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string UnidadeMedida { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorUnitario { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorTotal { get; set; }
    
    [MaxLength(500)]
    public string? Observacao { get; set; }
    
    [MaxLength(50)]
    public string? NumeroDocumento { get; set; }
    
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual Produto? Produto { get; set; }
    public virtual Ingrediente? Ingrediente { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Filial Filial { get; set; } = null!;
}