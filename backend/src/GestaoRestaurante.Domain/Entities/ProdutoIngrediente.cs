using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class ProdutoIngrediente
{
    public Guid ProdutoId { get; set; }
    
    public Guid IngredienteId { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantidade { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string UnidadeMedida { get; set; } = string.Empty;
    
    public DateTime DataVinculo { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual Produto Produto { get; set; } = null!;
    public virtual Ingrediente Ingrediente { get; set; } = null!;
}