using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetAllProdutos;

/// <summary>
/// Query para buscar todos os produtos ativos com filtros
/// </summary>
public class GetAllProdutosQuery : IQuery<IEnumerable<ProdutoDto>>
{
    public Guid? CategoriaId { get; set; }
    public bool? ProdutoVenda { get; set; }
    public bool? ProdutoEstoque { get; set; }
}