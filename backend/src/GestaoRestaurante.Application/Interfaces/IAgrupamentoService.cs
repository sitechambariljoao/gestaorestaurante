using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface IAgrupamentoService : IBaseService<Agrupamento, AgrupamentoDto, CreateAgrupamentoDto, UpdateAgrupamentoDto>
{
    Task<ServiceResult<IEnumerable<AgrupamentoDto>>> GetByFilialIdAsync(Guid filialId);
    Task<ServiceResult<bool>> ExistsByCodigoInEmpresaAsync(Guid empresaId, string codigo, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null);
}