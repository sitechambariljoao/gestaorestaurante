using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    
    public Guid PedidoId { get; set; }
    
    public Guid ProdutoId { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantidade { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoUnitario { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoTotal { get; set; }
    
    [MaxLength(500)]
    public string? Observacao { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual Pedido Pedido { get; set; } = null!;
    public virtual Produto Produto { get; set; } = null!;
}