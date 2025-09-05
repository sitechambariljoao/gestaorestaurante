using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.UpdatePreco;

/// <summary>
/// Command para atualizar preço de produto
/// </summary>
public class UpdatePrecoCommand : ICommand<ProdutoDto>
{
    public Guid Id { get; set; }
    public decimal NovoPreco { get; set; }

    public UpdatePrecoCommand(Guid id, decimal novoPreco)
    {
        Id = id;
        NovoPreco = novoPreco;
    }
}