using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Interfaces;

public interface IProdutoService : IBaseService<Produto, ProdutoDto, CreateProdutoDto, UpdateProdutoDto>
{
    Task<ServiceResult<IEnumerable<ProdutoDto>>> GetByCategoriaIdAsync(Guid categoriaId);
    Task<ServiceResult<IEnumerable<ProdutoDto>>> GetProdutosVendaAsync();
    Task<ServiceResult<IEnumerable<ProdutoDto>>> GetProdutosEstoqueAsync();
    Task<ServiceResult<bool>> ExistsByCodigoAsync(string codigo, Guid? excludeId = null);
    Task<ServiceResult<bool>> ExistsByNomeInCategoriaAsync(Guid categoriaId, string nome, Guid? excludeId = null);
}