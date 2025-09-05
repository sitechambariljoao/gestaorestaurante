namespace GestaoRestaurante.API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string correlationIdHeader = "X-Correlation-ID";

        // Verificar se já existe um correlation ID na requisição
        if (!context.Request.Headers.TryGetValue(correlationIdHeader, out var correlationId) ||
            string.IsNullOrEmpty(correlationId))
        {
            // Gerar novo correlation ID se não existir
            correlationId = Guid.NewGuid().ToString();
        }

        // Adicionar ao context para uso em outros middlewares/controllers
        context.Items["CorrelationId"] = correlationId.ToString();

        // Adicionar ao header de resposta
        context.Response.Headers.TryAdd(correlationIdHeader, correlationId.ToString());

        // Configurar logger com correlation ID
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId.ToString()
        });

        await _next(context);
    }
}