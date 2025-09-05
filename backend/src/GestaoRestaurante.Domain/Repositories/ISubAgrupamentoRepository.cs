using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface ISubAgrupamentoRepository : IBaseRepository<SubAgrupamento>
{
    Task<IEnumerable<SubAgrupamento>> GetByAgrupamentoIdAsync(Guid agrupamentoId);
    Task<bool> ExistsByCodigoInAgrupamentoAsync(Guid agrupamentoId, string codigo, Guid? excludeId = null);
    Task<bool> ExistsByNomeInAgrupamentoAsync(Guid agrupamentoId, string nome, Guid? excludeId = null);
    Task<SubAgrupamento?> GetByIdWithCentrosCustoAsync(Guid id);
    Task<IEnumerable<SubAgrupamento>?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<SubAgrupamento>?> GetByNomeAsync(string nome);
}