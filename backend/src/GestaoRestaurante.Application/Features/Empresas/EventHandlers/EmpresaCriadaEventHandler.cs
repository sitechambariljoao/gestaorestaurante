using MediatR;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Events;
using GestaoRestaurante.Domain.Events;

namespace GestaoRestaurante.Application.Features.Empresas.EventHandlers;

/// <summary>
/// Handler para o evento de empresa criada
/// </summary>
public sealed class EmpresaCriadaEventHandler : INotificationHandler<DomainEventNotification<EmpresaCriadaEvent>>
{
    private readonly ILogger<EmpresaCriadaEventHandler> _logger;

    public EmpresaCriadaEventHandler(ILogger<EmpresaCriadaEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<EmpresaCriadaEvent> notification, CancellationToken cancellationToken)
    {
        var empresaCriada = notification.DomainEvent;
        
        _logger.LogInformation(
            "Empresa criada: {EmpresaId} - {RazaoSocial} ({NomeFantasia}) - CNPJ: {Cnpj}",
            empresaCriada.EmpresaId,
            empresaCriada.RazaoSocial,
            empresaCriada.NomeFantasia,
            empresaCriada.Cnpj
        );

        // Aqui podem ser adicionadas outras ações:
        // - Enviar email de boas-vindas
        // - Criar configurações padrão
        // - Registrar em auditoria
        // - Integrar com sistemas externos
        
        await Task.CompletedTask;
    }
}