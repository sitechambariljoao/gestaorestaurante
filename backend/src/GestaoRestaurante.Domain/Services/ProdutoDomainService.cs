using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio relacionadas a produtos e preços
/// </summary>
public class ProdutoDomainService : IProdutoDomainService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ProdutoDomainService(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result> ValidateProdutoCreationAsync(string codigo, string nome, Guid categoriaId)
    {
        var errors = new List<string>();

        // Validar categoria existe e está ativa
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null || !categoria.Ativa)
        {
            errors.Add("Categoria não encontrada ou inativa");
        }

        // Validar código único globalmente
        if (await _produtoRepository.ExistsByCodigoAsync(codigo))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Produto", "Código", codigo));
        }

        // Validar nome único dentro da categoria
        if (await _produtoRepository.ExistsByNomeInCategoriaAsync(categoriaId, nome))
        {
            errors.Add($"Já existe um produto com o nome '{nome}' nesta categoria");
        }

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    public async Task<Result> ValidateProdutoUpdateAsync(Guid produtoId, string codigo, string nome, Guid categoriaId)
    {
        var errors = new List<string>();

        // Validar categoria existe e está ativa
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null || !categoria.Ativa)
        {
            errors.Add("Categoria não encontrada ou inativa");
        }

        // Validar código único globalmente (excluindo o produto atual)
        if (await _produtoRepository.ExistsByCodigoAsync(codigo, produtoId))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Produto", "Código", codigo));
        }

        // Validar nome único dentro da categoria (excluindo o produto atual)
        if (await _produtoRepository.ExistsByNomeInCategoriaAsync(categoriaId, nome, produtoId))
        {
            errors.Add($"Já existe um produto com o nome '{nome}' nesta categoria");
        }

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    public Result<decimal> ValidatePriceChange(decimal currentPrice, decimal newPrice)
    {
        // Validar preço positivo
        if (newPrice <= 0)
        {
            return Result<decimal>.Failure("Preço deve ser maior que zero");
        }

        // Validar se não é uma variação muito grande (mais de 500% ou menos de 50%)
        var priceChangeRatio = newPrice / currentPrice;
        
        if (priceChangeRatio > 5.0m) // Aumento maior que 500%
        {
            return Result<decimal>.Failure("Aumento de preço muito elevado (máximo 500%)");
        }

        if (priceChangeRatio < 0.5m) // Redução maior que 50%
        {
            return Result<decimal>.Failure("Redução de preço muito elevada (máximo 50%)");
        }

        return Result<decimal>.Success(newPrice);
    }

    public async Task<Result> ValidateProdutoInactivationAsync(Guid produtoId)
    {
        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Produto"));
        }

        // Regras específicas podem ser implementadas aqui
        // Por exemplo: verificar se tem pedidos pendentes, estoque positivo, etc.

        return Result.Success();
    }

    public Result ValidateProdutoConfiguration(bool produtoVenda, bool produtoEstoque)
    {
        // Um produto deve ser pelo menos de venda OU de estoque
        if (!produtoVenda && !produtoEstoque)
        {
            return Result.Failure("Produto deve ser configurado para venda ou controle de estoque (ou ambos)");
        }

        return Result.Success();
    }

    public Result<decimal> CalculateProfitMargin(decimal costPrice, decimal salePrice)
    {
        if (costPrice <= 0)
        {
            return Result<decimal>.Failure("Preço de custo deve ser maior que zero");
        }

        if (salePrice <= 0)
        {
            return Result<decimal>.Failure("Preço de venda deve ser maior que zero");
        }

        if (salePrice < costPrice)
        {
            return Result<decimal>.Failure("Preço de venda não pode ser menor que o preço de custo");
        }

        // Margem = ((Preço de Venda - Preço de Custo) / Preço de Venda) * 100
        var margin = ((salePrice - costPrice) / salePrice) * 100;

        return Result<decimal>.Success(Math.Round(margin, 2));
    }
}