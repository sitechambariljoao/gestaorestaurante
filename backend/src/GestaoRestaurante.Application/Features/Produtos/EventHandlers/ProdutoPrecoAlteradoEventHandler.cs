using MediatR;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Events;
using GestaoRestaurante.Domain.Events;
using GestaoRestaurante.Domain.Services;

namespace GestaoRestaurante.Application.Features.Produtos.EventHandlers;

/// <summary>
/// Handler para o evento de alteração de preço do produto
/// </summary>
public sealed class ProdutoPrecoAlteradoEventHandler : INotificationHandler<DomainEventNotification<ProdutoPrecoAlteradoEvent>>
{
    private readonly ILogger<ProdutoPrecoAlteradoEventHandler> _logger;
    private readonly IProdutoDomainService _produtoDomainService;

    public ProdutoPrecoAlteradoEventHandler(
        ILogger<ProdutoPrecoAlteradoEventHandler> logger, 
        IProdutoDomainService produtoDomainService)
    {
        _logger = logger;
        _produtoDomainService = produtoDomainService;
    }

    public async Task Handle(DomainEventNotification<ProdutoPrecoAlteradoEvent> notification, CancellationToken cancellationToken)
    {
        var precoAlterado = notification.DomainEvent;
        
        // Calcular margem de lucro (assumindo que existe preço de custo)
        var margemLucro = _produtoDomainService.CalculateProfitMargin(
            precoAlterado.PrecoAnterior * 0.7m, // Simulando 70% como custo
            precoAlterado.PrecoNovo
        );

        _logger.LogInformation(
            "Preço alterado - Produto: {ProdutoId} ({Nome}) - De: {PrecoAnterior:C} Para: {PrecoNovo:C} - Margem: {Margem:F2}%",
            precoAlterado.ProdutoId,
            precoAlterado.Nome,
            precoAlterado.PrecoAnterior,
            precoAlterado.PrecoNovo,
            margemLucro.Value
        );

        // Verificar se a variação de preço é significativa
        var percentualVariacao = Math.Abs((precoAlterado.PrecoNovo - precoAlterado.PrecoAnterior) / precoAlterado.PrecoAnterior * 100);
        
        if (percentualVariacao > 20)
        {
            _logger.LogWarning(
                "Grande variação de preço detectada: {Percentual:F2}% - Produto: {Nome}",
                percentualVariacao,
                precoAlterado.Nome
            );
        }

        // Aqui podem ser adicionadas outras ações:
        // - Atualizar tabelas de preços
        // - Notificar gerentes sobre grandes variações
        // - Reprocessar custos
        // - Atualizar sistemas de PDV
        
        await Task.CompletedTask;
    }
}