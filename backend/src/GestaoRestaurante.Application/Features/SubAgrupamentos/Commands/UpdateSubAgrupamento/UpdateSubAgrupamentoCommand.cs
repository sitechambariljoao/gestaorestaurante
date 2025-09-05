using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.UpdateSubAgrupamento;

public record UpdateSubAgrupamentoCommand(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao
) : ICommand<SubAgrupamentoDto>;