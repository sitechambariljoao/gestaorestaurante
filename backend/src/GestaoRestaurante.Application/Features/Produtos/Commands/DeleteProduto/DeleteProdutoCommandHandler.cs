using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.DeleteProduto;

/// <summary>
/// Handler para desativar produto (soft delete)
/// </summary>
public class DeleteProdutoCommandHandler : ICommandHandler<DeleteProdutoCommand, bool>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<DeleteProdutoCommandHandler> _logger;

    public DeleteProdutoCommandHandler(
        IProdutoRepository produtoRepository,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<DeleteProdutoCommandHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.command.delete");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "delete",
            ["produto_id"] = request.Id.ToString()
        });

        try
        {
            // Buscar produto com dependências
            var produto = await _produtoRepository.GetByIdWithDependenciesAsync(request.Id).ConfigureAwaitOptimized();
            
            if (produto == null || !produto.Ativa)
            {
                _logger.LogWarning("Tentativa de deletar produto inexistente: {ProdutoId}", request.Id);
                return Result<bool>.Failure("Produto não encontrado");
            }

            // Verificar se há movimentações que impedem a exclusão
            if (produto.ItensPedido.Any() || produto.MovimentacoesEstoque.Any())
            {
                return Result<bool>.Failure("Não é possível desativar produto com histórico de pedidos ou movimentações de estoque");
            }

            // Soft delete
            produto.Deactivate();

            await _produtoRepository.UpdateAsync(produto).ConfigureAwaitOptimized();

            // Invalidar cache
            await InvalidateRelatedCacheAsync(request.Id).ConfigureAwaitOptimized();

            _logger.LogInformation("Produto desativado com sucesso: {ProdutoId}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar produto {ProdutoId}", request.Id);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "delete" });
            
            return Result<bool>.Failure($"Erro ao desativar produto: {ex.Message}");
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