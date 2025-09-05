using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface ICategoriaService : IBaseService<Categoria, CategoriaDto, CreateCategoriaDto, UpdateCategoriaDto>
{
    Task<ServiceResult<IEnumerable<CategoriaDto>>> GetByCentroCustoIdAsync(Guid centroCustoId);
    Task<ServiceResult<IEnumerable<CategoriaDto>>> GetByNivelAsync(int nivel);
    Task<ServiceResult<IEnumerable<CategoriaDto>>> GetFilhasByPaiIdAsync(Guid categoriaPaiId);
    Task<ServiceResult<IEnumerable<CategoriaDto>>> GetHierarquiaByCentroCustoAsync(Guid centroCustoId);
    Task<ServiceResult<bool>> ExistsByCodigoInCentroCustoAsync(Guid centroCustoId, string codigo, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInCentroCustoAsync(Guid centroCustoId, string nome, Guid? excludeId = null);
}