using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface ICategoriaRepository : IBaseRepository<Categoria>
{
    Task<IEnumerable<Categoria>> GetByCentroCustoIdAsync(Guid centroCustoId);
    Task<IEnumerable<Categoria>> GetByNivelAsync(int nivel);
    Task<IEnumerable<Categoria>> GetFilhasByPaiIdAsync(Guid categoriaPaiId);
    Task<bool> ExistsByCodigoInCentroCustoAsync(Guid centroCustoId, string codigo, Guid? excludeId = null);
    Task<bool> ExistsByNomeInCentroCustoAsync(Guid centroCustoId, string nome, Guid? excludeId = null);
    Task<Categoria?> GetByIdWithFilhasAsync(Guid id);
    Task<IEnumerable<Categoria>> GetHierarquiaByCentroCustoAsync(Guid centroCustoId);
    Task<IEnumerable<Categoria>?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Categoria>?> GetByNomeAsync(string nome);
    Task<IEnumerable<Categoria>> GetByCategoriaPaiIdAsync(Guid categoriaPaiId);
}