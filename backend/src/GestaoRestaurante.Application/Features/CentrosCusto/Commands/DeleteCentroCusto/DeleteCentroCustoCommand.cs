using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.DeleteCentroCusto;

public record DeleteCentroCustoCommand(Guid Id) : ICommand<bool>;