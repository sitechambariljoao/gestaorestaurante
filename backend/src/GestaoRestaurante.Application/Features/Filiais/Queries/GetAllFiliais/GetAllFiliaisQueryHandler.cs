using GestaoRestaurante.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;

namespace GestaoRestaurante.Application.Features.Filiais.Queries.GetAllFiliais;

/// <summary>
/// Handler para buscar todas as filiais ativas
/// </summary>
public class GetAllFiliaisQueryHandler : IRequestHandler<GetAllFiliaisQuery, Result<IEnumerable<FilialDto>>>
{
    private readonly IFilialRepository _filialRepository;
    private readonly IMapper _mapper;
    private readonly IApplicationMetrics _metrics;
    private readonly IPerformanceProfiler _profiler;
    private readonly ILogger<GetAllFiliaisQueryHandler> _logger;

    public GetAllFiliaisQueryHandler(
        IFilialRepository filialRepository,
        IMapper mapper,
        IApplicationMetrics metrics,
        IPerformanceProfiler profiler,
        ILogger<GetAllFiliaisQueryHandler> logger)
    {
        _filialRepository = filialRepository;
        _mapper = mapper;
        _metrics = metrics;
        _profiler = profiler;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<FilialDto>>> Handle(GetAllFiliaisQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("filial.query.get_all");
        _metrics.IncrementCounter("filial.queries", new Dictionary<string, string> { ["type"] = "get_all" });

        try
        {
            _logger.LogInformation("Executando query GetAllFiliais. EmpresaId: {EmpresaId}", request.EmpresaId);

            var filiais = request.EmpresaId.HasValue
                ? await _filialRepository.GetByEmpresaIdAsync(request.EmpresaId.Value)
                : await _filialRepository.GetAllAsync();

            var filiaisDto = _mapper.Map<List<FilialDto>>(filiais);

            _logger.LogInformation("Query GetAllFiliais executada com sucesso. {Count} filiais encontradas", filiaisDto.Count);
            _metrics.SetGauge("filial.query.result_count", filiaisDto.Count);

            return Result<IEnumerable<FilialDto>>.Success(filiaisDto);
        }
        catch (Exception ex)
        {
            _metrics.IncrementCounter("filial.query.errors", new Dictionary<string, string> { ["type"] = "get_all" });
            _logger.LogError(ex, "Erro ao executar query GetAllFiliais");
            return Result<IEnumerable<FilialDto>>.Failure($"Erro ao buscar filiais: {ex.Message}");
        }
    }
}