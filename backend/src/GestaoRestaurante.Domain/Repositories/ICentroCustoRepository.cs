using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface ICentroCustoRepository : IBaseRepository<CentroCusto>
{
    Task<IEnumerable<CentroCusto>> GetBySubAgrupamentoIdAsync(Guid subAgrupamentoId);
    Task<bool> ExistsByCodigoInSubAgrupamentoAsync(Guid subAgrupamentoId, string codigo, Guid? excludeId = null);
    Task<bool> ExistsByNomeInSubAgrupamentoAsync(Guid subAgrupamentoId, string nome, Guid? excludeId = null);
    Task<CentroCusto?> GetByIdWithCategoriasAsync(Guid id);
    Task<IEnumerable<CentroCusto>> GetByFilialIdAsync(Guid filialId);
    Task<IEnumerable<CentroCusto>?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<CentroCusto>?> GetByNomeAsync(string nome);
}