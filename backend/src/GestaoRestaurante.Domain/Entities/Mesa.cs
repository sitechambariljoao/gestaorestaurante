using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class Mesa
{
    public Guid Id { get; set; }
    
    public Guid FilialId { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Numero { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Descricao { get; set; }
    
    public int Capacidade { get; set; } = 1;
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "LIVRE"; // "LIVRE", "OCUPADA", "RESERVADA", "MANUTENCAO"
    
    public bool Ativa { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataUltimaAlteracao { get; set; }
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}