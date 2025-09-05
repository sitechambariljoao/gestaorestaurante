using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetAllSubAgrupamentos;

public class GetAllSubAgrupamentosQueryHandler : IQueryHandler<GetAllSubAgrupamentosQuery, IEnumerable<SubAgrupamentoDto>>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IMapper _mapper;

    public GetAllSubAgrupamentosQueryHandler(
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IMapper mapper)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<SubAgrupamentoDto>>> Handle(GetAllSubAgrupamentosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Entities.SubAgrupamento>? subAgrupamentos;

            if (request.AgrupamentoId.HasValue)
            {
                subAgrupamentos = await _subAgrupamentoRepository.GetByAgrupamentoIdAsync(request.AgrupamentoId.Value);
            }
            else
            {
                subAgrupamentos = await _subAgrupamentoRepository.GetAllAsync();
            }

            var subAgrupamentosDto = _mapper.Map<IEnumerable<SubAgrupamentoDto>>(subAgrupamentos ?? Enumerable.Empty<Domain.Entities.SubAgrupamento>());
            return Result<IEnumerable<SubAgrupamentoDto>>.Success(subAgrupamentosDto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<SubAgrupamentoDto>>.Failure($"Erro interno: {ex.Message}");
        }
    }
}