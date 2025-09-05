using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetSubAgrupamentoById;

public class GetSubAgrupamentoByIdQueryHandler : IQueryHandler<GetSubAgrupamentoByIdQuery, SubAgrupamentoDto>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IMapper _mapper;

    public GetSubAgrupamentoByIdQueryHandler(
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IMapper mapper)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _mapper = mapper;
    }

    public async Task<Result<SubAgrupamentoDto>> Handle(GetSubAgrupamentoByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(request.Id);
            
            if (subAgrupamento == null)
            {
                return Result<SubAgrupamentoDto>.Failure(BusinessRuleMessages.SUBAGRUPAMENTO_NAO_ENCONTRADO);
            }

            var subAgrupamentoDto = _mapper.Map<SubAgrupamentoDto>(subAgrupamento);
            return Result<SubAgrupamentoDto>.Success(subAgrupamentoDto);
        }
        catch (Exception ex)
        {
            return Result<SubAgrupamentoDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}