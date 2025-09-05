using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface IFilialRepository : IBaseRepository<Filial>
{
    Task<IEnumerable<Filial>> GetByEmpresaIdAsync(Guid empresaId);
    Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
    Task<bool> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null);
    Task<int> CountActiveByEmpresaIdAsync(Guid empresaId);
    Task<bool> IsUnicaFilialDaEmpresaAsync(Guid filialId);
    Task<Filial?> GetByCnpjAsync(string cnpj);
    Task<Filial?> GetByEmailAsync(string email);
}