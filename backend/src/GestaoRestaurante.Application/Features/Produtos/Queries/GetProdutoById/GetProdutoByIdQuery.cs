using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetProdutoById;

/// <summary>
/// Query para buscar produto por ID
/// </summary>
public class GetProdutoByIdQuery : IQuery<ProdutoDto>
{
    public Guid Id { get; set; }

    public GetProdutoByIdQuery(Guid id)
    {
        Id = id;
    }
}