using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.Categorias.Queries.GetCategoriaById;

public class GetCategoriaByIdQueryHandler : IQueryHandler<GetCategoriaByIdQuery, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;

    public GetCategoriaByIdQueryHandler(
        ICategoriaRepository categoriaRepository,
        IMapper mapper)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
    }

    public async Task<Result<CategoriaDto>> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.Id);
            
            if (categoria == null)
            {
                return Result<CategoriaDto>.Failure(BusinessRuleMessages.CATEGORIA_NAO_ENCONTRADA);
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Result<CategoriaDto>.Success(categoriaDto);
        }
        catch (Exception ex)
        {
            return Result<CategoriaDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}