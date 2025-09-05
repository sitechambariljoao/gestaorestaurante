using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Agrupamentos.Commands.CreateAgrupamento;

/// <summary>
/// Handler para criar agrupamento
/// </summary>
public class CreateAgrupamentoCommandHandler : ICommandHandler<CreateAgrupamentoCommand, AgrupamentoDto>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly CreateAgrupamentoDbValidator _dbValidator;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<CreateAgrupamentoCommandHandler> _logger;

    public CreateAgrupamentoCommandHandler(
        IAgrupamentoRepository agrupamentoRepository,
        CreateAgrupamentoDbValidator dbValidator,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<CreateAgrupamentoCommandHandler> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _dbValidator = dbValidator;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<AgrupamentoDto>> Handle(CreateAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("agrupamento.command.create");
        
        _metrics.IncrementCounter("agrupamento.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "create",
            ["filial_id"] = request.CreateDto.FilialId.ToString()
        });

        try
        {
            // Validação de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(request.CreateDto);

            if (!dbValidationResult.IsValid)
            {
                var errors = dbValidationResult.Errors.Select(e => e.ErrorMessage);
                return Result<AgrupamentoDto>.Failure(errors);
            }

            // Criar agrupamento
            var agrupamento = new Agrupamento(
                request.CreateDto.FilialId,
                request.CreateDto.Codigo,
                request.CreateDto.Nome,
                request.CreateDto.Descricao
            );

            // Persistir
            await _agrupamentoRepository.AddAsync(agrupamento).ConfigureAwaitOptimized();
            await _agrupamentoRepository.SaveChangesAsync().ConfigureAwaitOptimized();

            // Invalidar cache relacionado
            await InvalidateRelatedCacheAsync(agrupamento.FilialId).ConfigureAwaitOptimized();

            _logger.LogInformation("Agrupamento criado com sucesso: {AgrupamentoId} - {Nome}", agrupamento.Id, agrupamento.Nome);

            // Mapear para DTO
            var agrupamentoDto = _mapper.Map<AgrupamentoDto>(agrupamento);
            return Result<AgrupamentoDto>.Success(agrupamentoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar agrupamento: {Codigo} - {Nome}", request.CreateDto.Codigo, request.CreateDto.Nome);
            
            _metrics.IncrementCounter("agrupamento.errors", new Dictionary<string, string> { ["operation"] = "create" });
            
            return Result<AgrupamentoDto>.Failure($"Erro ao criar agrupamento: {ex.Message}");
        }
    }

    private async Task InvalidateRelatedCacheAsync(Guid filialId)
    {
        var cacheKeys = new[]
        {
            "agrupamentos:all:*", // Todos os filtros de listagem
            $"agrupamentos:filial:{filialId}",
            "agrupamentos:all:null:null" // Cache sem filtros
        };

        foreach (var key in cacheKeys)
        {
            await _cache.RemovePatternAsync(key);
        }
    }
}