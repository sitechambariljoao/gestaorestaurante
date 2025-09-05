using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.UpdateProduto;

/// <summary>
/// Command para atualizar produto
/// </summary>
public class UpdateProdutoCommand : ICommand<ProdutoDto>
{
    public Guid Id { get; set; }
    public UpdateProdutoDto UpdateDto { get; set; }

    public UpdateProdutoCommand(Guid id, UpdateProdutoDto updateDto)
    {
        Id = id;
        UpdateDto = updateDto;
    }
}