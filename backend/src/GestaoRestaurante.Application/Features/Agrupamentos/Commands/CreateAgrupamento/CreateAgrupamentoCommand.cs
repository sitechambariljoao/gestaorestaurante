using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.CreateAgrupamento;

/// <summary>
/// Command para criar agrupamento
/// </summary>
public class CreateAgrupamentoCommand : ICommand<AgrupamentoDto>
{
    public CreateAgrupamentoDto CreateDto { get; set; }

    public CreateAgrupamentoCommand(CreateAgrupamentoDto createDto)
    {
        CreateDto = createDto;
    }
}