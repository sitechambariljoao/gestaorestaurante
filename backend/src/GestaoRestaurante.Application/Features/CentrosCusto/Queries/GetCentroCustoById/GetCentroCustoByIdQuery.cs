using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetCentroCustoById;

public record GetCentroCustoByIdQuery(Guid Id) : IQuery<CentroCustoDto>;