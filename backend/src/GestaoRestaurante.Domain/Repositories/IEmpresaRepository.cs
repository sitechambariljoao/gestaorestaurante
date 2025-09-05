using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Repositories;

public interface IEmpresaRepository : IBaseRepository<Empresa>
{
    Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<Empresa>> GetAllWithFiliaisAsync();
    Task<Empresa?> GetByIdWithFiliaisAsync(Guid id);
}