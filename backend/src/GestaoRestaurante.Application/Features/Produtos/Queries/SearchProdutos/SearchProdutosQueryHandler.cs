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

namespace GestaoRestaurante.Application.Features.Produtos.Queries.SearchProdutos;

/// <summary>
/// Handler para buscar produtos por termo
/// </summary>
public class SearchProdutosQueryHandler : IQueryHandler<SearchProdutosQuery, IEnumerable<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<SearchProdutosQueryHandler> _logger;

    public SearchProdutosQueryHandler(
        IProdutoRepository produtoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<SearchProdutosQueryHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProdutoDto>>> Handle(SearchProdutosQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.query.search");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "search",
            ["term_length"] = request.Termo.Length.ToString(),
            ["has_venda_filter"] = request.ProdutoVenda.HasValue.ToString()
        });

        try
        {
            // Validações
            if (string.IsNullOrWhiteSpace(request.Termo))
            {
                return Result<IEnumerable<ProdutoDto>>.Failure("Termo de busca é obrigatório");
            }

            if (request.Termo.Length < 2)
            {
                return Result<IEnumerable<ProdutoDto>>.Failure("Termo de busca deve ter pelo menos 2 caracteres");
            }

            // Cache mais curto para buscas (dados podem mudar frequentemente)
            var cacheKey = $"produtos:search:{request.Termo}:{request.ProdutoVenda}:{request.Limite}";
            
            var cachedResult = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var produtos = await _produtoRepository.SearchAsync(
                        request.Termo, 
                        request.ProdutoVenda, 
                        request.Limite
                    ).ConfigureAwaitOptimized();

                    return produtos.Select(p => _mapper.Map<ProdutoDto>(p)).ToList();
                },
                TimeSpan.FromMinutes(2) // Cache mais curto para buscas
            ).ConfigureAwaitOptimized();

            _metrics.RecordValue("produto.search.result_count", cachedResult.Count());
            
            return Result<IEnumerable<ProdutoDto>>.Success(cachedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos com termo: {Termo}", request.Termo);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "search" });
            
            return Result<IEnumerable<ProdutoDto>>.Failure($"Erro ao buscar produtos: {ex.Message}");
        }
    }
}