using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.DeleteSubAgrupamento;

public record DeleteSubAgrupamentoCommand(Guid Id) : ICommand<bool>;