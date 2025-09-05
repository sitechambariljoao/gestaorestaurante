using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetProdutoById;

/// <summary>
/// Handler para buscar produto por ID
/// </summary>
public class GetProdutoByIdQueryHandler : IQueryHandler<GetProdutoByIdQuery, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<GetProdutoByIdQueryHandler> _logger;

    public GetProdutoByIdQueryHandler(
        IProdutoRepository produtoRepository,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<GetProdutoByIdQueryHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<ProdutoDto>> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.query.get_by_id");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "get_by_id",
            ["produto_id"] = request.Id.ToString()
        });

        try
        {
            var cacheKey = $"produto:id:{request.Id}";

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.

#pragma warning disable CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).

            var cachedResult = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var produto = await _produtoRepository.GetByIdWithDetailsAsync(request.Id).ConfigureAwaitOptimized();
                    
                    if (produto == null || !produto.Ativa)
                    {
                        return null;
                    }

                    return _mapper.Map<ProdutoDto>(produto);
                },
                TimeSpan.FromMinutes(10)
            ).ConfigureAwaitOptimized();
#pragma warning restore CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.



            if (cachedResult == null)
            {
                _logger.LogWarning("Produto não encontrado: {ProdutoId}", request.Id);
                return Result<ProdutoDto>.Failure("Produto não encontrado");
            }
            
            return Result<ProdutoDto>.Success(cachedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por ID: {ProdutoId}", request.Id);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "get_by_id" });
            
            return Result<ProdutoDto>.Failure($"Erro ao buscar produto: {ex.Message}");
        }
    }
}