using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface IProdutoRepository : IBaseRepository<Produto>
{
    Task<IEnumerable<Produto>> GetByCategoriaIdAsync(Guid categoriaId);
    Task<bool> ExistsByCodigoAsync(string codigo, Guid? excludeId = null);
    Task<bool> ExistsByNomeInCategoriaAsync(Guid categoriaId, string nome, Guid? excludeId = null);
    Task<IEnumerable<Produto>> GetProdutosVendaAsync();
    Task<IEnumerable<Produto>> GetProdutosEstoqueAsync();
    Task<Produto?> GetByIdWithIngredientesAsync(Guid id);
    
    // Métodos para CQRS
    Task<IEnumerable<Produto>> GetFilteredAsync(Guid? categoriaId, bool? produtoVenda, bool? produtoEstoque);
    Task<Produto?> GetByIdWithDetailsAsync(Guid id);
    Task<Produto?> GetByIdWithDependenciesAsync(Guid id);
    Task<IEnumerable<Produto>> SearchAsync(string termo, bool? produtoVenda, int limite = 50);
}