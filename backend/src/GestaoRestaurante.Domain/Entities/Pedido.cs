using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    
    public Guid FilialId { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    public Guid? MesaId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string NumeroPedido { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "ABERTO"; // "ABERTO", "FECHADO", "CANCELADO"
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotal { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Desconto { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorPago { get; set; }
    
    [MaxLength(50)]
    public string? FormaPagamento { get; set; }
    
    [MaxLength(500)]
    public string? Observacao { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataFechamento { get; set; }
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Mesa? Mesa { get; set; }
    public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}