using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Categorias.Queries.GetAllCategorias;

public class GetAllCategoriasQuery : IQuery<IEnumerable<CategoriaDto>>
{
    public Guid? CentroCustoId { get; set; }
    public int? Nivel { get; set; }
}