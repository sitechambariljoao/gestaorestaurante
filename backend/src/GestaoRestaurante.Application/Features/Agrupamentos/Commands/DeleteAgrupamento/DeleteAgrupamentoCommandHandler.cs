using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.DeleteAgrupamento;

/// <summary>
/// Handler para desativar agrupamento (soft delete)
/// </summary>
public class DeleteAgrupamentoCommandHandler : ICommandHandler<DeleteAgrupamentoCommand, bool>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<DeleteAgrupamentoCommandHandler> _logger;

    public DeleteAgrupamentoCommandHandler(
        IAgrupamentoRepository agrupamentoRepository,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<DeleteAgrupamentoCommandHandler> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("agrupamento.command.delete");
        
        _metrics.IncrementCounter("agrupamento.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "delete",
            ["agrupamento_id"] = request.Id.ToString()
        });

        try
        {
            // Buscar agrupamento com dependências
            var agrupamento = await _agrupamentoRepository.GetByIdWithDependenciesAsync(request.Id).ConfigureAwaitOptimized();
            
            if (agrupamento == null || !agrupamento.Ativa)
            {
                _logger.LogWarning("Tentativa de deletar agrupamento inexistente: {AgrupamentoId}", request.Id);
                return Result<bool>.Failure("Agrupamento não encontrado");
            }

            // Verificar se há dependências que impedem a exclusão
            if (agrupamento.SubAgrupamentos.Any(sa => sa.Ativa))
            {
                return Result<bool>.Failure("Não é possível desativar agrupamento que possui sub-agrupamentos ativos");
            }

            // Soft delete
            agrupamento.Desativar();

            await _agrupamentoRepository.UpdateAsync(agrupamento).ConfigureAwaitOptimized();

            // Invalidar cache
            await InvalidateRelatedCacheAsync(request.Id, agrupamento.FilialId).ConfigureAwaitOptimized();

            _logger.LogInformation("Agrupamento desativado com sucesso: {AgrupamentoId}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar agrupamento {AgrupamentoId}", request.Id);
            
            _metrics.IncrementCounter("agrupamento.errors", new Dictionary<string, string> { ["operation"] = "delete" });
            
            return Result<bool>.Failure($"Erro ao desativar agrupamento: {ex.Message}");
        }
    }

    private async Task InvalidateRelatedCacheAsync(Guid agrupamentoId, Guid filialId)
    {
        var cacheKeys = new[]
        {
            $"agrupamento:id:{agrupamentoId}",
            "agrupamentos:all:*",
            $"agrupamentos:filial:{filialId}"
        };

        foreach (var key in cacheKeys)
        {
            await _cache.RemovePatternAsync(key);
        }
    }
}