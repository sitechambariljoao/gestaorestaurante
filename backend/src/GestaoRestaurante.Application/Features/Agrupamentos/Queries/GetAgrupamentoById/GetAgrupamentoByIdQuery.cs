using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAgrupamentoById;

/// <summary>
/// Query para buscar agrupamento por ID
/// </summary>
public class GetAgrupamentoByIdQuery : IQuery<AgrupamentoDto>
{
    public Guid Id { get; set; }

    public GetAgrupamentoByIdQuery(Guid id)
    {
        Id = id;
    }
}