using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAllAgrupamentos;

/// <summary>
/// Query para buscar todos os agrupamentos com filtros opcionais
/// </summary>
public class GetAllAgrupamentosQuery : IQuery<IEnumerable<AgrupamentoDto>>
{
    public Guid? FilialId { get; set; }
}