using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Extensions;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.UpdateProduto;

/// <summary>
/// Handler para atualizar produto
/// </summary>
public class UpdateProdutoCommandHandler : ICommandHandler<UpdateProdutoCommand, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly UpdateProdutoDbValidator _dbValidator;
    private readonly IMapper _mapper;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<UpdateProdutoCommandHandler> _logger;

    public UpdateProdutoCommandHandler(
        IProdutoRepository produtoRepository,
        UpdateProdutoDbValidator dbValidator,
        IMapper mapper,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<UpdateProdutoCommandHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _dbValidator = dbValidator;
        _mapper = mapper;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<ProdutoDto>> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
    {
        using var measurement = _profiler.StartMeasurement("produto.command.update");
        
        _metrics.IncrementCounter("produto.operations", new Dictionary<string, string> 
        { 
            ["operation"] = "update",
            ["produto_id"] = request.Id.ToString()
        });

        try
        {
            // Buscar produto existente
            var produto = await _produtoRepository.GetByIdAsync(request.Id).ConfigureAwaitOptimized();
            
            if (produto == null || !produto.Ativa)
            {
                _logger.LogWarning("Tentativa de atualizar produto inexistente: {ProdutoId}", request.Id);
                return Result<ProdutoDto>.Failure("Produto não encontrado");
            }

            // Validação de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(request.UpdateDto, cancellationToken);

            if (!dbValidationResult.IsValid)
            {
                var errors = dbValidationResult.Errors.Select(e => e.ErrorMessage);
                return Result<ProdutoDto>.Failure(errors);
            }

            // Validar preço
            if (request.UpdateDto.Preco <= 0)
            {
                return Result<ProdutoDto>.Failure("Preço deve ser maior que zero");
            }

            // Atualizar apenas os campos permitidos
            produto.UpdateDetails(
                request.UpdateDto.Codigo, 
                request.UpdateDto.Nome, 
                request.UpdateDto.Descricao,
                request.UpdateDto.Preco,
                request.UpdateDto.UnidadeMedida,
                request.UpdateDto.ProdutoVenda,
                request.UpdateDto.ProdutoEstoque
            );

            await _produtoRepository.UpdateAsync(produto).ConfigureAwaitOptimized();

            // Invalidar cache
            await InvalidateRelatedCacheAsync(request.Id).ConfigureAwaitOptimized();

            var result = _mapper.Map<ProdutoDto>(produto);
            return Result<ProdutoDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", request.Id);
            
            _metrics.IncrementCounter("produto.errors", new Dictionary<string, string> { ["operation"] = "update" });
            
            return Result<ProdutoDto>.Failure($"Erro ao atualizar produto: {ex.Message}");
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