using GestaoRestaurante.Domain.Events;
using GestaoRestaurante.Domain.Aggregates;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.Entities;

public class Produto : BaseEntity, IAggregateRoot
{
    public Guid CategoriaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public bool ProdutoVenda { get; set; } = false; // Para cardápio
    public bool ProdutoEstoque { get; set; } = false; // Para controle de estoque
    
    // Versão para controle de concorrência otimista
    public long Version { get; private set; } = 1;
    
    // Relacionamentos
    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<ProdutoIngrediente> Ingredientes { get; set; } = new List<ProdutoIngrediente>();
    public virtual ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
    public virtual ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();

    // Construtores
    public Produto() { } // Para EF Core e Seeder

    public Produto(Guid categoriaId, string codigo, string nome, decimal preco, string unidadeMedida, 
                   string? descricao = null, bool produtoVenda = false, bool produtoEstoque = false)
    {
        AtualizarDados(categoriaId, codigo, nome, preco, unidadeMedida, descricao, produtoVenda, produtoEstoque);
        
        // Disparar evento de criação
        AddDomainEvent(new ProdutoCriadoEvent(Id, categoriaId, codigo, nome, preco, unidadeMedida));
    }

    // Métodos de domínio
    public void AtualizarDados(Guid categoriaId, string codigo, string nome, decimal preco, string unidadeMedida, 
                               string? descricao = null, bool produtoVenda = false, bool produtoEstoque = false)
    {
        ValidarDados(codigo, nome, preco, unidadeMedida);
        
        CategoriaId = categoriaId;
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Preco = preco;
        UnidadeMedida = unidadeMedida.Trim().ToUpperInvariant();
        Descricao = descricao?.Trim();
        ProdutoVenda = produtoVenda;
        ProdutoEstoque = produtoEstoque;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero", nameof(novoPreco));
        
        var precoAnterior = Preco;
        Preco = novoPreco;
        AtualizarTimestamp();
        IncrementVersion();
        
        // Disparar evento de alteração de preço
        if (precoAnterior != novoPreco)
        {
            AddDomainEvent(new ProdutoPrecoAlteradoEvent(Id, Nome, precoAnterior, novoPreco));
        }
    }

    public void AtivarParaVenda()
    {
        ProdutoVenda = true;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void DesativarParaVenda()
    {
        ProdutoVenda = false;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void AtivarControleEstoque()
    {
        ProdutoEstoque = true;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void DesativarControleEstoque()
    {
        ProdutoEstoque = false;
        AtualizarTimestamp();
        IncrementVersion();
    }

    // Métodos para CQRS
    public void UpdateDetails(string codigo, string nome, string? descricao, decimal preco, 
                             string unidadeMedida, bool produtoVenda, bool produtoEstoque)
    {
        ValidarDados(codigo, nome, preco, unidadeMedida);
        
        Codigo = codigo.Trim().ToUpperInvariant();
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        Preco = preco;
        UnidadeMedida = unidadeMedida.Trim().ToUpperInvariant();
        ProdutoVenda = produtoVenda;
        ProdutoEstoque = produtoEstoque;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void Deactivate()
    {
        Ativa = false;
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void UpdatePrice(decimal novoPreco)
    {
        AtualizarPreco(novoPreco);
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
        
        if (string.IsNullOrWhiteSpace(UnidadeMedida))
            errors.Add("Unidade de Medida é obrigatória");
        
        if (CategoriaId == Guid.Empty)
            errors.Add("Categoria é obrigatória");

        // Validar tamanhos
        if (Codigo?.Length > 20)
            errors.Add("Código deve ter no máximo 20 caracteres");
        
        if (Nome?.Length > 100)
            errors.Add("Nome deve ter no máximo 100 caracteres");
        
        if (UnidadeMedida?.Length > 10)
            errors.Add("Unidade de Medida deve ter no máximo 10 caracteres");
        
        if (Descricao?.Length > 500)
            errors.Add("Descrição deve ter no máximo 500 caracteres");

        // Validar regras de negócio
        if (Preco <= 0)
            errors.Add("Preço deve ser maior que zero");
        
        if (Preco > 999999.99m)
            errors.Add("Preço deve ser menor que R$ 999.999,99");

        // Validar regras de negócio específicas
        ValidateBusinessRules(errors);

        if (errors.Count > 0)
            throw new DomainValidationException($"Produto inválido: {string.Join(", ", errors)}");
    }

    private void ValidateBusinessRules(List<string> errors)
    {
        // Se produto for para venda, deve ter preço válido
        if (ProdutoVenda && Preco <= 0)
            errors.Add("Produto para venda deve ter preço válido");

        // Se produto tiver ingredientes, deve estar ativo
        if (Ingredientes.Count > 0 && !Ativa)
            errors.Add("Produto com ingredientes deve estar ativo");
    }

    private void IncrementVersion()
    {
        Version++;
    }

    private static void ValidarDados(string codigo, string nome, decimal preco, string unidadeMedida)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo, nameof(codigo));
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
        ArgumentException.ThrowIfNullOrWhiteSpace(unidadeMedida, nameof(unidadeMedida));
        
        if (preco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero", nameof(preco));
    }
}