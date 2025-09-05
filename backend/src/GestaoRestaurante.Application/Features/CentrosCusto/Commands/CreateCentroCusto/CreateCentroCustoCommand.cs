using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.CreateCentroCusto;

public record CreateCentroCustoCommand(
    Guid SubAgrupamentoId,
    string Codigo,
    string Nome,
    string? Descricao
) : ICommand<CentroCustoDto>;