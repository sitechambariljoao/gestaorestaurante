using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Empresas.Queries.GetAllEmpresas;

/// <summary>
/// Query para obter todas as empresas
/// </summary>
public sealed record GetAllEmpresasQuery : IQuery<IEnumerable<EmpresaDto>>;