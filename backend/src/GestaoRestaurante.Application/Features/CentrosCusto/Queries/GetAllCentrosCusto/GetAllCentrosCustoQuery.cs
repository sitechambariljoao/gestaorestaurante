using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetAllCentrosCusto;

public class GetAllCentrosCustoQuery : IQuery<IEnumerable<CentroCustoDto>>
{
    public Guid? SubAgrupamentoId { get; set; }
}