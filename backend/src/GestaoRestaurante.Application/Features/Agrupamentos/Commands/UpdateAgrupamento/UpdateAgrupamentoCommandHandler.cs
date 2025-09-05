using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.UpdateAgrupamento;

/// <summary>
/// Handler para atualizar agrupamento
/// </summary>
public class UpdateAgrupamentoCommandHandler : ICommandHandler<UpdateAgrupamentoCommand, AgrupamentoDto>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly UpdateAgrupamentoDbValidator _dbValidator;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<UpdateAgrupamentoCommandHandler> _logger;

    public UpdateAgrupamentoCommandHandler(
        IAgrupamentoRepository agrupamentoRepository,
        UpdateAgrupamentoDbValidator dbValidator,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<UpdateAgrupamentoCommandHandler> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _dbValidator = dbValidator;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<AgrupamentoDto>> Handle(UpdateAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("agrupamento.command.update");
        
        _metrics.IncrementCounter("agrupamento.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "update",
            ["agrupamento_id"] = request.Id.ToString()
        });

        try
        {
            // Buscar agrupamento existente
            var agrupamento = await _agrupamentoRepository.GetByIdAsync(request.Id).ConfigureAwaitOptimized();
            
            if (agrupamento == null || !agrupamento.Ativa)
            {
                _logger.LogWarning("Tentativa de atualizar agrupamento inexistente: {AgrupamentoId}", request.Id);
                return Result<AgrupamentoDto>.Failure("Agrupamento não encontrado");
            }

            // Validação de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(request.UpdateDto, cancellationToken);

            if (!dbValidationResult.IsValid)
            {
                var errors = dbValidationResult.Errors.Select(e => e.ErrorMessage);
                return Result<AgrupamentoDto>.Failure(errors);
            }

            // Atualizar dados
            agrupamento.AtualizarDados(
                agrupamento.FilialId,
                request.UpdateDto.Codigo, 
                request.UpdateDto.Nome, 
                request.UpdateDto.Descricao
            );

            await _agrupamentoRepository.UpdateAsync(agrupamento).ConfigureAwaitOptimized();

            // Invalidar cache
            await InvalidateRelatedCacheAsync(request.Id, agrupamento.FilialId).ConfigureAwaitOptimized();

            _logger.LogInformation("Agrupamento atualizado com sucesso: {AgrupamentoId} - {Nome}", request.Id, agrupamento.Nome);

            var result = _mapper.Map<AgrupamentoDto>(agrupamento);
            return Result<AgrupamentoDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar agrupamento {AgrupamentoId}", request.Id);
            
            _metrics.IncrementCounter("agrupamento.errors", new Dictionary<string, string> { ["operation"] = "update" });
            
            return Result<AgrupamentoDto>.Failure($"Erro ao atualizar agrupamento: {ex.Message}");
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