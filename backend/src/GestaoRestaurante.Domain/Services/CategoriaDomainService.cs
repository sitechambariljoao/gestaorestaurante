using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio relacionadas à hierarquia de categorias
/// </summary>
public class CategoriaDomainService : ICategoriaDomainService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IProdutoRepository _produtoRepository;

    public CategoriaDomainService(ICategoriaRepository categoriaRepository, IProdutoRepository produtoRepository)
    {
        _categoriaRepository = categoriaRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<Result> ValidateCategoriaHierarchyAsync(int nivel, Guid? categoriaPaiId, Guid centroCustoId)
    {
        // Validar nível dentro do permitido (1-3)
        if (nivel < ApplicationConstants.BusinessRules.CategoriaMinLevel || 
            nivel > ApplicationConstants.BusinessRules.CategoriaMaxLevel)
        {
            return Result.Failure($"Nível da categoria deve estar entre {ApplicationConstants.BusinessRules.CategoriaMinLevel} e {ApplicationConstants.BusinessRules.CategoriaMaxLevel}");
        }

        // Categoria de nível 1 não deve ter pai
        if (nivel == 1 && categoriaPaiId.HasValue)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.CannotHaveParent, 
                "Categoria", "1", "categoria pai"));
        }

        // Categoria de nível > 1 deve ter pai
        if (nivel > 1 && !categoriaPaiId.HasValue)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.MustHaveParent, 
                "Categoria", nivel.ToString(), "categoria pai"));
        }

        // Se tem categoria pai, validar hierarquia
        if (categoriaPaiId.HasValue)
        {
            var categoriaPai = await _categoriaRepository.GetByIdAsync(categoriaPaiId.Value);
            if (categoriaPai == null)
            {
                return Result.Failure("Categoria pai não encontrada");
            }

            // Validar se o nível é consecutivo
            if (nivel != categoriaPai.Nivel + 1)
            {
                return Result.Failure($"Categoria de nível {nivel} deve ter pai de nível {nivel - 1}");
            }

            // Validar se pertence ao mesmo centro de custo
            if (categoriaPai.CentroCustoId != centroCustoId)
            {
                return Result.Failure("Categoria deve pertencer ao mesmo centro de custo da categoria pai");
            }
        }

        return Result.Success();
    }

    public async Task<Result> ValidateCategoriaRemovalAsync(Guid categoriaId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Categoria"));
        }

        // Verificar se tem categorias filhas
        var categoriasFilhas = await _categoriaRepository.GetFilhasByPaiIdAsync(categoriaId);
        if (categoriasFilhas.Any(c => c.Ativa))
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.CannotDeleteWithDependents, 
                "Categoria", "categorias filhas"));
        }

        // Verificar se tem produtos vinculados
        var produtos = await _produtoRepository.GetByCategoriaIdAsync(categoriaId);
        if (produtos.Any(p => p.Ativa))
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.CannotDeleteWithDependents, 
                "Categoria", "produtos"));
        }

        return Result.Success();
    }

    public Result<int> CalculateNextLevel(Categoria? categoriaPai)
    {
        if (categoriaPai == null)
        {
            return Result<int>.Success(1); // Categoria raiz
        }

        var nextLevel = categoriaPai.Nivel + 1;
        
        if (nextLevel > ApplicationConstants.BusinessRules.CategoriaMaxLevel)
        {
            return Result<int>.Failure($"Não é possível criar categoria de nível {nextLevel}. Máximo permitido: {ApplicationConstants.BusinessRules.CategoriaMaxLevel}");
        }

        return Result<int>.Success(nextLevel);
    }

    public async Task<Result> ValidateCategoriaLevelChangeAsync(Guid categoriaId, int newLevel, Guid? newCategoriaPaiId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Categoria"));
        }

        // Não permitir mudança se tem categorias filhas
        var categoriasFilhas = await _categoriaRepository.GetFilhasByPaiIdAsync(categoriaId);
        if (categoriasFilhas.Any(c => c.Ativa))
        {
            return Result.Failure("Não é possível alterar o nível de categoria que possui categorias filhas");
        }

        // Validar nova hierarquia
        return await ValidateCategoriaHierarchyAsync(newLevel, newCategoriaPaiId, categoria.CentroCustoId);
    }

    public async Task<Result<string>> GetCategoriaPathAsync(Guid categoriaId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null)
        {
            return Result<string>.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Categoria"));
        }

        var path = new List<string> { categoria.Nome };

        // Subir na hierarquia até chegar na raiz
        var currentCategoria = categoria;
        while (currentCategoria.CategoriaPaiId.HasValue)
        {
            var parent = await _categoriaRepository.GetByIdAsync(currentCategoria.CategoriaPaiId.Value);
            if (parent == null) break;

            path.Insert(0, parent.Nome);
            currentCategoria = parent;
        }

        return Result<string>.Success(string.Join(" > ", path));
    }
}