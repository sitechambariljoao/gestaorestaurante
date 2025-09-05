using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio relacionadas à hierarquia de categorias
/// </summary>
public interface ICategoriaDomainService
{
    /// <summary>
    /// Valida se uma categoria pode ser criada na hierarquia especificada
    /// </summary>
    Task<Result> ValidateCategoriaHierarchyAsync(int nivel, Guid? categoriaPaiId, Guid centroCustoId);

    /// <summary>
    /// Valida se uma categoria pode ser removida (não tem filhas ou produtos)
    /// </summary>
    Task<Result> ValidateCategoriaRemovalAsync(Guid categoriaId);

    /// <summary>
    /// Calcula o próximo nível disponível para uma categoria filha
    /// </summary>
    Result<int> CalculateNextLevel(Categoria? categoriaPai);

    /// <summary>
    /// Valida se uma categoria pode mudar de nível na hierarquia
    /// </summary>
    Task<Result> ValidateCategoriaLevelChangeAsync(Guid categoriaId, int newLevel, Guid? newCategoriaPaiId);

    /// <summary>
    /// Obtém o caminho completo da categoria na hierarquia
    /// </summary>
    Task<Result<string>> GetCategoriaPathAsync(Guid categoriaId);
}