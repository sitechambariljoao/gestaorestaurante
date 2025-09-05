using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.DeleteFilial;

public record DeleteFilialCommand(Guid Id) : ICommand<bool>;