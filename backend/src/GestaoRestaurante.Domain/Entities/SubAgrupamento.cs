namespace GestaoRestaurante.Domain.Entities;

public class SubAgrupamento : BaseEntity
{
    public Guid AgrupamentoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Relacionamentos
    public virtual Agrupamento Agrupamento { get; set; } = null!;
    public virtual ICollection<CentroCusto> CentrosCusto { get; set; } = new List<CentroCusto>();

    // Construtores
    public SubAgrupamento() { } // Para EF Core e Seeder

    public SubAgrupamento(Guid agrupamentoId, string codigo, string nome, string? descricao = null)
    {
        AtualizarDados(agrupamentoId, codigo, nome, descricao);
    }

    // Métodos de domínio
    public void AtualizarDados(Guid agrupamentoId, string codigo, string nome, string? descricao = null)
    {
        ValidarDados(codigo, nome);
        
        AgrupamentoId = agrupamentoId;
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        AtualizarTimestamp();
    }

    public bool PodeSerExcluido() => !CentrosCusto.Any(cc => cc.Ativa);

    private static void ValidarDados(string codigo, string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
    }
}