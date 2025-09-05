namespace GestaoRestaurante.Domain.Entities;

public class Agrupamento : BaseEntity
{
    public Guid FilialId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Relacionamentos
    public virtual Filial Filial { get; set; } = null!;
    public virtual ICollection<SubAgrupamento> SubAgrupamentos { get; set; } = new List<SubAgrupamento>();

    // Construtores
    public Agrupamento() { } // Para EF Core e Seeder

    public Agrupamento(Guid filialId, string codigo, string nome, string? descricao = null)
    {
        AtualizarDados(filialId, codigo, nome, descricao);
    }

    // Métodos de domínio
    public void AtualizarDados(Guid filialId, string codigo, string nome, string? descricao = null)
    {
        ValidarDados(codigo, nome);
        
        FilialId = filialId;
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        AtualizarTimestamp();
    }

    public bool PodeSerExcluido() => !SubAgrupamentos.Any(sa => sa.Ativa);

    // Método para CQRS
    public void Desativar()
    {
        Ativa = false;
        AtualizarTimestamp();
    }

    private static void ValidarDados(string codigo, string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
    }
}