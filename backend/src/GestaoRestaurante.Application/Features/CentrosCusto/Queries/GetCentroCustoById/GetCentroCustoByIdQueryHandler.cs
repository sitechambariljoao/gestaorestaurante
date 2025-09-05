using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetCentroCustoById;

public class GetCentroCustoByIdQueryHandler : IQueryHandler<GetCentroCustoByIdQuery, CentroCustoDto>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly IMapper _mapper;

    public GetCentroCustoByIdQueryHandler(
        ICentroCustoRepository centroCustoRepository,
        IMapper mapper)
    {
        _centroCustoRepository = centroCustoRepository;
        _mapper = mapper;
    }

    public async Task<Result<CentroCustoDto>> Handle(GetCentroCustoByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var centroCusto = await _centroCustoRepository.GetByIdAsync(request.Id);
            
            if (centroCusto == null)
            {
                return Result<CentroCustoDto>.Failure(BusinessRuleMessages.CENTRO_CUSTO_NAO_ENCONTRADO);
            }

            var centroCustoDto = _mapper.Map<CentroCustoDto>(centroCusto);
            return Result<CentroCustoDto>.Success(centroCustoDto);
        }
        catch (Exception ex)
        {
            return Result<CentroCustoDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}