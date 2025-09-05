using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetAllProdutos;

/// <summary>
/// Handler para buscar todos os produtos com filtros e performance otimizada
/// </summary>
public class GetAllProdutosQueryHandler : IQueryHandler<GetAllProdutosQuery, IEnumerable<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllProdutosQueryHandler> _logger;

    public GetAllProdutosQueryHandler(
        IProdutoRepository produtoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<GetAllProdutosQueryHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProdutoDto>>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.query.get_all");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "get_all",
            ["has_categoria_filter"] = request.CategoriaId.HasValue.ToString(),
            ["has_venda_filter"] = request.ProdutoVenda.HasValue.ToString(),
            ["has_estoque_filter"] = request.ProdutoEstoque.HasValue.ToString()
        });

        try
        {
            // Cache key baseado nos filtros
            var cacheKey = $"produtos:all:{request.CategoriaId}:{request.ProdutoVenda}:{request.ProdutoEstoque}";
            
            var cachedResult = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var produtos = await _produtoRepository.GetFilteredAsync(
                        request.CategoriaId, 
                        request.ProdutoVenda, 
                        request.ProdutoEstoque
                    ).ConfigureAwaitOptimized();

                    return produtos.Select(p => _mapper.Map<ProdutoDto>(p)).ToList();
                },
                TimeSpan.FromMinutes(5) // Cache mais curto para dados dinâmicos
            ).ConfigureAwaitOptimized();

            _metrics.RecordValue("produto.query.result_count", cachedResult.Count());
            
            return Result<IEnumerable<ProdutoDto>>.Success(cachedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos com filtros: CategoriaId={CategoriaId}, ProdutoVenda={ProdutoVenda}, ProdutoEstoque={ProdutoEstoque}",
                request.CategoriaId, request.ProdutoVenda, request.ProdutoEstoque);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "get_all" });
            
            return Result<IEnumerable<ProdutoDto>>.Failure($"Erro ao buscar produtos: {ex.Message}");
        }
    }
}