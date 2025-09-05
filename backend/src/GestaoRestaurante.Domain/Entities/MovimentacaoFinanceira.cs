using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class MovimentacaoFinanceira
{
    public Guid Id { get; set; }
    
    public Guid FilialId { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    public Guid? PedidoId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TipoMovimentacao { get; set; } = string.Empty; // "ENTRADA", "SAIDA"
    
    [Required]
    [MaxLength(100)]
    public string Categoria { get; set; } = string.Empty; // "VENDAS", "COMPRAS", "DESPESAS", etc.
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? FormaPagamento { get; set; }
    
    [MaxLength(50)]
    public string? NumeroDocumento { get; set; }
    
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;
    
    public DateTime? DataVencimento { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "PENDENTE"; // "PENDENTE", "PAGO", "CANCELADO"
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Pedido? Pedido { get; set; }
}