using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Filiais.Queries.GetAllFiliais;

/// <summary>
/// Query para buscar todas as filiais ativas
/// </summary>
public class GetAllFiliaisQuery : IQuery<IEnumerable<FilialDto>>
{
    public Guid? EmpresaId { get; set; }
}