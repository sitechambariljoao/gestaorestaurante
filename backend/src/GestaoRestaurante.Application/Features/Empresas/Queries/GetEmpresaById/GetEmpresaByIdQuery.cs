using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Empresas.Queries.GetEmpresaById;

/// <summary>
/// Query para obter uma empresa por ID
/// </summary>
public sealed record GetEmpresaByIdQuery(Guid Id) : IQuery<EmpresaDto>;