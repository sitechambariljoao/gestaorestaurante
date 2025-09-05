using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Filiais.Queries.GetFilialById;

public record GetFilialByIdQuery(Guid Id) : IQuery<FilialDto>;