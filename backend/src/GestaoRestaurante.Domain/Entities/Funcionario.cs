using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoRestaurante.Domain.Entities;

public class Funcionario
{
    public Guid Id { get; set; }
    
    public Guid FilialId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(18)]
    public string Cpf { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Rg { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? Telefone { get; set; }
    
    [MaxLength(500)]
    public string? Endereco { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Cargo { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Salario { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? PercentualComissao { get; set; }
    
    public DateTime DataAdmissao { get; set; }
    
    public DateTime? DataDemissao { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataUltimaAlteracao { get; set; }
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual ICollection<RegistroJornada> RegistrosJornada { get; set; } = new List<RegistroJornada>();
}