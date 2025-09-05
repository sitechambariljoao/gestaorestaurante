using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.CreateCategoria;

public record CreateCategoriaCommand(
    Guid CentroCustoId,
    Guid? CategoriaPaiId,
    string Codigo,
    string Nome,
    string? Descricao,
    int Nivel
) : ICommand<CategoriaDto>;