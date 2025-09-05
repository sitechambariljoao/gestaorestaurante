using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.UpdateAgrupamento;

/// <summary>
/// Command para atualizar agrupamento
/// </summary>
public class UpdateAgrupamentoCommand : ICommand<AgrupamentoDto>
{
    public Guid Id { get; set; }
    public UpdateAgrupamentoDto UpdateDto { get; set; }

    public UpdateAgrupamentoCommand(Guid id, UpdateAgrupamentoDto updateDto)
    {
        Id = id;
        UpdateDto = updateDto;
    }
}