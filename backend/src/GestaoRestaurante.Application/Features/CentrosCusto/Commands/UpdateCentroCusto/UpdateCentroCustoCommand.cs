using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.UpdateCentroCusto;

public record UpdateCentroCustoCommand(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao
) : ICommand<CentroCustoDto>;