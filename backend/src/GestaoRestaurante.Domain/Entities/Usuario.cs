using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GestaoRestaurante.Domain.Entities;

public class Usuario : IdentityUser<Guid>
{
    public Guid EmpresaId { get; set; }
    public Guid FilialId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(18)]
    public string? Cpf { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Perfil { get; set; } = "USUARIO"; // "ADMIN", "GERENTE", "USUARIO"
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataUltimaAlteracao { get; set; }
    
    public DateTime? UltimoLogin { get; set; }
    
    // Relacionamentos
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual Filial Filial { get; set; } = null!;
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public virtual ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
    public virtual ICollection<LogOperacao> LogsOperacao { get; set; } = new List<LogOperacao>();
}