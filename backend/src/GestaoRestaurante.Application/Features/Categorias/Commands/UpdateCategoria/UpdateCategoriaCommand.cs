using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.UpdateCategoria;

public record UpdateCategoriaCommand(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao
) : ICommand<CategoriaDto>;