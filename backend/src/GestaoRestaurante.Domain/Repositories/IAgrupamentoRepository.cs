using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface IAgrupamentoRepository : IBaseRepository<Agrupamento>
{
    Task<IEnumerable<Agrupamento>> GetByFilialIdAsync(Guid filialId);
    Task<bool> ExistsByCodigoInEmpresaAsync(Guid empresaId, string codigo, Guid? excludeId = null);
    Task<bool> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null);
    Task<Agrupamento?> GetByIdWithSubAgrupamentosAsync(Guid id);
    Task<Agrupamento?> GetByIdWithFiliaisAsync(Guid id);
    
    // Método para CQRS
    Task<Agrupamento?> GetByIdWithDependenciesAsync(Guid id);
}