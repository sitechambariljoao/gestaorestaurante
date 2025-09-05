using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface ICentroCustoService : IBaseService<CentroCusto, CentroCustoDto, CreateCentroCustoDto, UpdateCentroCustoDto>
{
    Task<ServiceResult<IEnumerable<CentroCustoDto>>> GetBySubAgrupamentoIdAsync(Guid subAgrupamentoId);
    Task<ServiceResult<IEnumerable<CentroCustoDto>>> GetByFilialIdAsync(Guid filialId);
    Task<ServiceResult<bool>> ExistsByCodigoInSubAgrupamentoAsync(Guid subAgrupamentoId, string codigo, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInSubAgrupamentoAsync(Guid subAgrupamentoId, string nome, Guid? excludeId = null);
}