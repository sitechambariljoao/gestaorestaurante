using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.Categorias.Queries.GetAllCategorias;

public class GetAllCategoriasQueryHandler : IQueryHandler<GetAllCategoriasQuery, IEnumerable<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriasQueryHandler(
        ICategoriaRepository categoriaRepository,
        IMapper mapper)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CategoriaDto>>> Handle(GetAllCategoriasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Entities.Categoria>? categorias;

            if (request.CentroCustoId.HasValue && request.Nivel.HasValue)
            {
                if (request.Nivel.Value < 1 || request.Nivel.Value > 3)
                {
                    return Result<IEnumerable<CategoriaDto>>.Failure("Nível deve estar entre 1 e 3");
                }

                var categoriasNoCentroCusto = await _categoriaRepository.GetByCentroCustoIdAsync(request.CentroCustoId.Value);
                categorias = categoriasNoCentroCusto?.Where(c => c.Nivel == request.Nivel.Value && c.Ativa) ?? Enumerable.Empty<Domain.Entities.Categoria>();
            }
            else if (request.CentroCustoId.HasValue)
            {
                categorias = await _categoriaRepository.GetByCentroCustoIdAsync(request.CentroCustoId.Value);
                categorias = categorias?.Where(c => c.Ativa) ?? Enumerable.Empty<Domain.Entities.Categoria>();
            }
            else if (request.Nivel.HasValue)
            {
                if (request.Nivel.Value < 1 || request.Nivel.Value > 3)
                {
                    return Result<IEnumerable<CategoriaDto>>.Failure("Nível deve estar entre 1 e 3");
                }
                categorias = await _categoriaRepository.GetByNivelAsync(request.Nivel.Value);
            }
            else
            {
                categorias = await _categoriaRepository.GetAllAsync();
            }

            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias ?? Enumerable.Empty<Domain.Entities.Categoria>());
            return Result<IEnumerable<CategoriaDto>>.Success(categoriasDto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CategoriaDto>>.Failure($"Erro interno: {ex.Message}");
        }
    }
}