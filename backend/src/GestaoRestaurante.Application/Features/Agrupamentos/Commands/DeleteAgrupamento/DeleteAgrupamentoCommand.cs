using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.DeleteAgrupamento;

/// <summary>
/// Command para desativar agrupamento (soft delete)
/// </summary>
public class DeleteAgrupamentoCommand : ICommand<bool>
{
    public Guid Id { get; set; }

    public DeleteAgrupamentoCommand(Guid id)
    {
        Id = id;
    }
}