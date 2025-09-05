using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAllAgrupamentos;

/// <summary>
/// Handler para buscar todos os agrupamentos com filtros e performance otimizada
/// </summary>
public class GetAllAgrupamentosQueryHandler : IQueryHandler<GetAllAgrupamentosQuery, IEnumerable<AgrupamentoDto>>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllAgrupamentosQueryHandler> _logger;

    public GetAllAgrupamentosQueryHandler(
        IAgrupamentoRepository agrupamentoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<GetAllAgrupamentosQueryHandler> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AgrupamentoDto>>> Handle(GetAllAgrupamentosQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("agrupamento.query.get_all");
        
        _metrics.IncrementCounter("agrupamento.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "get_all",
            ["has_filial_filter"] = request.FilialId.HasValue.ToString()
        });

        try
        {
            // Validar que apenas um filtro seja fornecido
            if (request.FilialId.HasValue)
            {
                return Result<IEnumerable<AgrupamentoDto>>.Failure("Informe apenas empresaId ou filialId, não ambos");
            }

            // Cache key baseado nos filtros
            var cacheKey = $"agrupamentos:all:{request.FilialId}";
            
            var cachedResult = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    IEnumerable<Domain.Entities.Agrupamento> agrupamentos;

                    if (request.FilialId.HasValue)
                    {
                        agrupamentos = await _agrupamentoRepository.GetByFilialIdAsync(request.FilialId.Value).ConfigureAwaitOptimized();
                    }
                    else
                    {
                        agrupamentos = await _agrupamentoRepository.GetAllAsync().ConfigureAwaitOptimized();
                    }

                    return agrupamentos.Select(a => _mapper.Map<AgrupamentoDto>(a)).ToList();
                },
                TimeSpan.FromMinutes(10) // Cache por 10 minutos
            ).ConfigureAwaitOptimized();

            _metrics.RecordValue("agrupamento.query.result_count", cachedResult.Count());
            
            return Result<IEnumerable<AgrupamentoDto>>.Success(cachedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar agrupamentos com filtro: FilialId={FilialId}", request.FilialId);
            
            _metrics.IncrementCounter("agrupamento.errors", new Dictionary<string, string> { ["operation"] = "get_all" });
            
            return Result<IEnumerable<AgrupamentoDto>>.Failure($"Erro ao buscar agrupamentos: {ex.Message}");
        }
    }
}