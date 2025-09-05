using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.UpdatePreco;

/// <summary>
/// Handler para atualizar preço de produto
/// </summary>
public class UpdatePrecoCommandHandler : ICommandHandler<UpdatePrecoCommand, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<UpdatePrecoCommandHandler> _logger;

    public UpdatePrecoCommandHandler(
        IProdutoRepository produtoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<UpdatePrecoCommandHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<ProdutoDto>> Handle(UpdatePrecoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.command.update_preco");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "update_preco",
            ["produto_id"] = request.Id.ToString(),
            ["novo_preco"] = request.NovoPreco.ToString("F2")
        });

        try
        {
            // Validar preço
            if (request.NovoPreco <= 0)
            {
                return Result<ProdutoDto>.Failure("Preço deve ser maior que zero");
            }

            // Buscar produto existente
            var produto = await _produtoRepository.GetByIdAsync(request.Id).ConfigureAwaitOptimized();
            
            if (produto == null || !produto.Ativa)
            {
                _logger.LogWarning("Tentativa de atualizar preço de produto inexistente: {ProdutoId}", request.Id);
                return Result<ProdutoDto>.Failure("Produto não encontrado");
            }

            var precoAnterior = produto.Preco;

            // Atualizar preço
            produto.UpdatePrice(request.NovoPreco);

            await _produtoRepository.UpdateAsync(produto).ConfigureAwaitOptimized();

            // Invalidar cache
            await InvalidateRelatedCacheAsync(request.Id).ConfigureAwaitOptimized();

            _logger.LogInformation("Preço do produto {ProdutoId} atualizado de {PrecoAnterior} para {NovoPreco}", 
                request.Id, precoAnterior, request.NovoPreco);

            var result = _mapper.Map<ProdutoDto>(produto);
            return Result<ProdutoDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar preço do produto {ProdutoId}", request.Id);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "update_preco" });
            
            return Result<ProdutoDto>.Failure($"Erro ao atualizar preço: {ex.Message}");
        }
    }

    private async Task InvalidateRelatedCacheAsync(Guid produtoId)
    {
        var cacheKeys = new[]
        {
            $"produto:id:{produtoId}",
            "produtos:all:*",
            "produtos:search:*"
        };

        foreach (var key in cacheKeys)
        {
            await _cache.RemovePatternAsync(key);
        }
    }
}