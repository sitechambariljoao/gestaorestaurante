using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface IFilialService : IBaseService<Filial, FilialDto, CreateFilialDto, UpdateFilialDto>
{
    Task<ServiceResult<IEnumerable<FilialDto>>> GetByEmpresaAsync(Guid empresaId);
    Task<ServiceResult<bool>> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null);
    Task<ServiceResult<bool>> IsUnicaFilialDaEmpresaAsync(Guid filialId);
}