using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Categorias.Queries.GetCategoriaById;

public record GetCategoriaByIdQuery(Guid Id) : IQuery<CategoriaDto>;