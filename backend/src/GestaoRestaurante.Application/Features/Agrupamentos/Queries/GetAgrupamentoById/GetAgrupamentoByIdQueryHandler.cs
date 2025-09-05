using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAgrupamentoById;

/// <summary>
/// Handler para buscar agrupamento por ID
/// </summary>
public class GetAgrupamentoByIdQueryHandler : IQueryHandler<GetAgrupamentoByIdQuery, AgrupamentoDto>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAgrupamentoByIdQueryHandler> _logger;

    public GetAgrupamentoByIdQueryHandler(
        IAgrupamentoRepository agrupamentoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<GetAgrupamentoByIdQueryHandler> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<AgrupamentoDto>> Handle(GetAgrupamentoByIdQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("agrupamento.query.get_by_id");
        
        _metrics.IncrementCounter("agrupamento.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "get_by_id",
            ["agrupamento_id"] = request.Id.ToString()
        });

        try
        {
            var cacheKey = $"agrupamento:id:{request.Id}";
            
            var cachedResult = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var agrupamento = await _agrupamentoRepository.GetByIdAsync(request.Id).ConfigureAwaitOptimized();
                    
                    if (agrupamento == null || !agrupamento.Ativa)
                    {
                        return null;
                    }

                    return _mapper.Map<AgrupamentoDto>(agrupamento);
                },
                TimeSpan.FromMinutes(15) // Cache mais longo para entidades específicas
            ).ConfigureAwaitOptimized();

            if (cachedResult == null)
            {
                _logger.LogWarning("Agrupamento não encontrado: {AgrupamentoId}", request.Id);
                return Result<AgrupamentoDto>.Failure("Agrupamento não encontrado");
            }
            
            return Result<AgrupamentoDto>.Success(cachedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar agrupamento por ID: {AgrupamentoId}", request.Id);
            
            _metrics.IncrementCounter("agrupamento.errors", new Dictionary<string, string> { ["operation"] = "get_by_id" });
            
            return Result<AgrupamentoDto>.Failure($"Erro ao buscar agrupamento: {ex.Message}");
        }
    }
}