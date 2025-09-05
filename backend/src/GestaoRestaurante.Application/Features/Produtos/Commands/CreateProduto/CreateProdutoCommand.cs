using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.CreateProduto;

/// <summary>
/// Command para criar um novo produto
/// </summary>
public sealed record CreateProdutoCommand(
    Guid CategoriaId,
    string Codigo,
    string Nome,
    string? Descricao,
    decimal Preco,
    string UnidadeMedida,
    bool ProdutoVenda,
    bool ProdutoEstoque
) : ICommand<ProdutoDto>;