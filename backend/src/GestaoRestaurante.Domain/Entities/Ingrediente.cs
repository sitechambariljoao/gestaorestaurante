using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class Ingrediente
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string UnidadeMedida { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal EstoqueAtual { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal EstoqueMinimo { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CustoUnitario { get; set; } = 0;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataUltimaAlteracao { get; set; }
    
    // Relacionamentos
    public virtual ICollection<ProdutoIngrediente> Produtos { get; set; } = new List<ProdutoIngrediente>();
    public virtual ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
}