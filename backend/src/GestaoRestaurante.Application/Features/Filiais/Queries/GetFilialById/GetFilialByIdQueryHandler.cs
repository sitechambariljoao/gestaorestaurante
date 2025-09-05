using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.Filiais.Queries.GetFilialById;

public class GetFilialByIdQueryHandler : IQueryHandler<GetFilialByIdQuery, FilialDto>
{
    private readonly IFilialRepository _filialRepository;
    private readonly IMapper _mapper;

    public GetFilialByIdQueryHandler(
        IFilialRepository filialRepository,
        IMapper mapper)
    {
        _filialRepository = filialRepository;
        _mapper = mapper;
    }

    public async Task<Result<FilialDto>> Handle(GetFilialByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filial = await _filialRepository.GetByIdAsync(request.Id);
            
            if (filial == null)
            {
                return Result<FilialDto>.Failure(BusinessRuleMessages.FILIAL_NAO_ENCONTRADA);
            }

            var filialDto = _mapper.Map<FilialDto>(filial);
            return Result<FilialDto>.Success(filialDto);
        }
        catch (Exception ex)
        {
            return Result<FilialDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}