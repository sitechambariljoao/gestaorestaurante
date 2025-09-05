using GestaoRestaurante.Domain.Aggregates;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.Entities;

public class Categoria : BaseEntity, IAggregateRoot
{
    public Guid CentroCustoId { get; set; }
    public Guid? CategoriaPaiId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Nivel { get; set; } // 1, 2 ou 3
    
    // Versão para controle de concorrência otimista
    public long Version { get; private set; } = 1;
    
    // Relacionamentos
    public virtual CentroCusto CentroCusto { get; set; } = null!;
    public virtual Categoria? CategoriaPai { get; set; }
    public virtual ICollection<Categoria> CategoriasFilhas { get; set; } = new List<Categoria>();
    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    // Construtores
    public Categoria() { } // Para EF Core e Seeder

    public Categoria(Guid centroCustoId, string codigo, string nome, int nivel, Guid? categoriaPaiId = null, string? descricao = null)
    {
        AtualizarDados(centroCustoId, codigo, nome, nivel, categoriaPaiId, descricao);
    }

    // Métodos de domínio
    public void AtualizarDados(Guid centroCustoId, string codigo, string nome, int nivel, Guid? categoriaPaiId = null, string? descricao = null)
    {
        ValidarDados(codigo, nome, nivel);
        ValidarHierarquia(nivel, categoriaPaiId);
        
        CentroCustoId = centroCustoId;
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Nivel = nivel;
        CategoriaPaiId = categoriaPaiId;
        Descricao = descricao?.Trim();
        AtualizarTimestamp();
        IncrementVersion();
    }

    public bool PodeSerExcluida() => !CategoriasFilhas.Any() && !Produtos.Any();

    public bool EhCategoriaRaiz() => Nivel == 1 && CategoriaPaiId == null;

    public bool EhCategoriaFolha() => !CategoriasFilhas.Any();

    private static void ValidarDados(string codigo, string nome, int nivel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
        
        if (nivel < 1 || nivel > 3)
            throw new ArgumentException("Nível deve estar entre 1 e 3", nameof(nivel));
    }

    private static void ValidarHierarquia(int nivel, Guid? categoriaPaiId)
    {
        if (nivel == 1 && categoriaPaiId.HasValue)
            throw new ArgumentException("Categoria de nível 1 não pode ter categoria pai");
        
        if (nivel > 1 && !categoriaPaiId.HasValue)
            throw new ArgumentException($"Categoria de nível {nivel} deve ter uma categoria pai");
    }

    // Implementação do IAggregateRoot
    public void ValidateInvariants()
    {
        var errors = new List<string>();

        // Validar dados obrigatórios
        if (string.IsNullOrWhiteSpace(Codigo))
            errors.Add("Código é obrigatório");
        
        if (string.IsNullOrWhiteSpace(Nome))
            errors.Add("Nome é obrigatório");
        
        if (CentroCustoId == Guid.Empty)
            errors.Add("Centro de Custo é obrigatório");

        // Validar tamanhos
        if (Codigo?.Length > 20)
            errors.Add("Código deve ter no máximo 20 caracteres");
        
        if (Nome?.Length > 100)
            errors.Add("Nome deve ter no máximo 100 caracteres");
        
        if (Descricao?.Length > 500)
            errors.Add("Descrição deve ter no máximo 500 caracteres");

        // Validar níveis hierárquicos
        if (Nivel < 1 || Nivel > 3)
            errors.Add("Nível deve estar entre 1 e 3");

        // Validar regras de negócio específicas
        ValidateBusinessRules(errors);

        if (errors.Count > 0)
            throw new DomainValidationException($"Categoria inválida: {string.Join(", ", errors)}");
    }

    private void ValidateBusinessRules(List<string> errors)
    {
        // Categoria nível 1 não pode ter pai
        if (Nivel == 1 && CategoriaPaiId.HasValue)
            errors.Add("Categoria de nível 1 não pode ter categoria pai");
        
        // Categoria nível 2 ou 3 deve ter pai
        if (Nivel > 1 && !CategoriaPaiId.HasValue)
            errors.Add($"Categoria de nível {Nivel} deve ter uma categoria pai");

        // Se categoria tem produtos, deve estar ativa
        if (Produtos.Count > 0 && !Ativa)
            errors.Add("Categoria com produtos deve estar ativa");

        // Se categoria tem subcategorias, deve estar ativa
        if (CategoriasFilhas.Count > 0 && !Ativa)
            errors.Add("Categoria com subcategorias deve estar ativa");

        // Verificar hierarquia circular (se categoria pai está definida)
        if (CategoriaPai != null && CategoriaPai.Id == Id)
            errors.Add("Categoria não pode ser pai de si mesma");
    }

    private void IncrementVersion()
    {
        Version++;
    }
}