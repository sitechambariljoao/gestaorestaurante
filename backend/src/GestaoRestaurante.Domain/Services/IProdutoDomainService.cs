using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio relacionadas a produtos e preços
/// </summary>
public interface IProdutoDomainService
{
    /// <summary>
    /// Valida se um produto pode ser criado com o código especificado
    /// </summary>
    Task<Result> ValidateProdutoCreationAsync(string codigo, string nome, Guid categoriaId);

    /// <summary>
    /// Valida se um produto pode ser atualizado
    /// </summary>
    Task<Result> ValidateProdutoUpdateAsync(Guid produtoId, string codigo, string nome, Guid categoriaId);

    /// <summary>
    /// Calcula se um produto pode ter seu preço alterado
    /// </summary>
    Result<decimal> ValidatePriceChange(decimal currentPrice, decimal newPrice);

    /// <summary>
    /// Valida se um produto pode ser inativado
    /// </summary>
    Task<Result> ValidateProdutoInactivationAsync(Guid produtoId);

    /// <summary>
    /// Valida configuração de produto para venda e estoque
    /// </summary>
    Result ValidateProdutoConfiguration(bool produtoVenda, bool produtoEstoque);

    /// <summary>
    /// Calcula margem de lucro baseada no custo e preço de venda
    /// </summary>
    Result<decimal> CalculateProfitMargin(decimal costPrice, decimal salePrice);
}