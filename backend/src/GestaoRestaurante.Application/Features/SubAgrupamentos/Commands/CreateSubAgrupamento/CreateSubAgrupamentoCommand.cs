using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.CreateSubAgrupamento;

public record CreateSubAgrupamentoCommand(
    Guid AgrupamentoId,
    string Codigo,
    string Nome,
    string? Descricao
) : ICommand<SubAgrupamentoDto>;