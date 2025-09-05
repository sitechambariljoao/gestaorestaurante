using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetProdutosBusca;

/// <summary>
/// Query para busca complexa de produtos usando specifications
/// </summary>
public sealed record GetProdutosBuscaQuery(
    string? TermoBusca = null,
    Guid? CategoriaId = null,
    decimal? PrecoMinimo = null,
    decimal? PrecoMaximo = null,
    bool ApenasAtivos = true,
    bool? ApenasVenda = null
) : IQuery<IEnumerable<ProdutoDto>>;