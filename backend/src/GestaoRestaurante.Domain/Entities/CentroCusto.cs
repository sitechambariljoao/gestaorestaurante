namespace GestaoRestaurante.Domain.Entities;

public class CentroCusto : BaseEntity
{
    public Guid SubAgrupamentoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Relacionamentos
    public virtual SubAgrupamento SubAgrupamento { get; set; } = null!;
    public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    // Construtores
    public CentroCusto() { } // Para EF Core e Seeder

    public CentroCusto(Guid subAgrupamentoId, string codigo, string nome, string? descricao = null)
    {
        AtualizarDados(subAgrupamentoId, codigo, nome, descricao);
    }

    // Métodos de domínio
    public void AtualizarDados(Guid subAgrupamentoId, string codigo, string nome, string? descricao = null)
    {
        ValidarDados(codigo, nome);
        
        SubAgrupamentoId = subAgrupamentoId;
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        AtualizarTimestamp();
    }

    public bool PodeSerExcluido() => !Categorias.Any(c => c.Ativa);

    private static void ValidarDados(string codigo, string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
    }
}