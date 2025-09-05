using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface IEmpresaService : IBaseService<Empresa, EmpresaDto, CreateEmpresaDto, UpdateEmpresaDto>
{
    Task<ServiceResult<IEnumerable<FilialDto>>> GetFiliaisAsync(Guid empresaId);
    Task<ServiceResult<bool>> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByEmailAsync(string email, Guid? excludeId = null);
}