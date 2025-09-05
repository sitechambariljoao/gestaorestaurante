using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface ISubAgrupamentoService : IBaseService<SubAgrupamento, SubAgrupamentoDto, CreateSubAgrupamentoDto, UpdateSubAgrupamentoDto>
{
    Task<ServiceResult<IEnumerable<SubAgrupamentoDto>>> GetByAgrupamentoIdAsync(Guid agrupamentoId);
    Task<ServiceResult<bool>> ExistsByCodigoInAgrupamentoAsync(Guid agrupamentoId, string codigo, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInAgrupamentoAsync(Guid agrupamentoId, string nome, Guid? excludeId = null);
}