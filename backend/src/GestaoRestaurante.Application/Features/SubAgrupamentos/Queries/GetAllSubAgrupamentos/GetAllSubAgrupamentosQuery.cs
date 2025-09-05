using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetAllSubAgrupamentos;

public class GetAllSubAgrupamentosQuery : IQuery<IEnumerable<SubAgrupamentoDto>>
{
    public Guid? AgrupamentoId { get; set; }
}