using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetAllCentrosCusto;

public class GetAllCentrosCustoQueryHandler : IQueryHandler<GetAllCentrosCustoQuery, IEnumerable<CentroCustoDto>>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly IMapper _mapper;

    public GetAllCentrosCustoQueryHandler(
        ICentroCustoRepository centroCustoRepository,
        IMapper mapper)
    {
        _centroCustoRepository = centroCustoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CentroCustoDto>>> Handle(GetAllCentrosCustoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Entities.CentroCusto>? centrosCusto;

            if (request.SubAgrupamentoId.HasValue)
            {
                centrosCusto = await _centroCustoRepository.GetBySubAgrupamentoIdAsync(request.SubAgrupamentoId.Value);
            }
            else
            {
                centrosCusto = await _centroCustoRepository.GetAllAsync();
            }

            var centrosCustoDto = _mapper.Map<IEnumerable<CentroCustoDto>>(centrosCusto ?? Enumerable.Empty<Domain.Entities.CentroCusto>());
            return Result<IEnumerable<CentroCustoDto>>.Success(centrosCustoDto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CentroCustoDto>>.Failure($"Erro interno: {ex.Message}");
        }
    }
}