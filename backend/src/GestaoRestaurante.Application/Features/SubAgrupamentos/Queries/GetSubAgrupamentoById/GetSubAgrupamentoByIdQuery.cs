using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetSubAgrupamentoById;

public record GetSubAgrupamentoByIdQuery(Guid Id) : IQuery<SubAgrupamentoDto>;