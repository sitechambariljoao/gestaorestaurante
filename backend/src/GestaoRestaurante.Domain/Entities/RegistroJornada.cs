using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Domain.Entities;

public class RegistroJornada
{
    public Guid Id { get; set; }
    
    public Guid FuncionarioId { get; set; }
    
    public DateTime DataRegistro { get; set; }
    
    public DateTime? HorarioEntrada { get; set; }
    
    public DateTime? HorarioSaida { get; set; }
    
    public DateTime? HorarioInicioIntervalo { get; set; }
    
    public DateTime? HorarioFimIntervalo { get; set; }
    
    public int? MinutosTrabalhados { get; set; }
    
    public int? MinutosExtras { get; set; }
    
    [MaxLength(500)]
    public string? Observacoes { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual Funcionario Funcionario { get; set; } = null!;
}