using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.DeleteProduto;

/// <summary>
/// Command para desativar produto (soft delete)
/// </summary>
public class DeleteProdutoCommand : ICommand<bool>
{
    public Guid Id { get; set; }

    public DeleteProdutoCommand(Guid id)
    {
        Id = id;
    }
}