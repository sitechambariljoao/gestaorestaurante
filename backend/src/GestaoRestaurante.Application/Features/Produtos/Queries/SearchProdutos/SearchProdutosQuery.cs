using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.SearchProdutos;

/// <summary>
/// Query para buscar produtos por termo
/// </summary>
public class SearchProdutosQuery : IQuery<IEnumerable<ProdutoDto>>
{
    public string Termo { get; set; } = string.Empty;
    public bool? ProdutoVenda { get; set; }
    public int Limite { get; set; } = 50;

    public SearchProdutosQuery(string termo, bool? produtoVenda = null, int limite = 50)
    {
        Termo = termo;
        ProdutoVenda = produtoVenda;
        Limite = limite;
    }
}