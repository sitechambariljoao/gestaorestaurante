using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.DeleteCategoria;

public record DeleteCategoriaCommand(Guid Id) : ICommand<bool>;