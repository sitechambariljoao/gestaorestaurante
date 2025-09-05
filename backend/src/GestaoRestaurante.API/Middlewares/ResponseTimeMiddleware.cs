using System.Diagnostics;

namespace GestaoRestaurante.API.Middlewares;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseTimeMiddleware> _logger;

    public ResponseTimeMiddleware(RequestDelegate next, ILogger<ResponseTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Adicionar header de início
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            // Adicionar header com tempo de resposta
            context.Response.Headers.TryAdd("X-Response-Time", $"{elapsedMs}ms");
            
            // Log de performance
            LogPerformance(context, elapsedMs);
            
            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }
    }

    private void LogPerformance(HttpContext context, long elapsedMs)
    {
        var request = context.Request;
        var response = context.Response;
        var requestId = context.Items["RequestId"]?.ToString() ?? "N/A";

        var performanceLog = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path,
            StatusCode = response.StatusCode,
            ElapsedMs = elapsedMs,
            Timestamp = DateTime.UtcNow
        };

        // Definir nível de log baseado no tempo de resposta
        var logLevel = GetPerformanceLogLevel(elapsedMs, response.StatusCode);
        
        _logger.Log(logLevel, "Performance: {Method} {Path} respondeu em {ElapsedMs}ms com status {StatusCode} [{RequestId}]",
            request.Method, request.Path, elapsedMs, response.StatusCode, requestId);

        // Log adicional para requisições muito lentas
        if (elapsedMs > 5000) // Mais de 5 segundos
        {
            _logger.LogWarning("Requisição muito lenta detectada: {@PerformanceLog}", performanceLog);
        }
        else if (elapsedMs > 2000) // Mais de 2 segundos
        {
            _logger.LogInformation("Requisição lenta detectada: {@PerformanceLog}", performanceLog);
        }

        // Métricas detalhadas apenas em debug
        _logger.LogDebug("Métricas detalhadas: {@PerformanceLog}", performanceLog);
    }

    private static LogLevel GetPerformanceLogLevel(long elapsedMs, int statusCode)
    {
        // Se houver erro, sempre log como erro independente do tempo
        if (statusCode >= 500)
            return LogLevel.Error;
            
        if (statusCode >= 400)
            return LogLevel.Warning;

        // Baseado no tempo de resposta
        return elapsedMs switch
        {
            > 5000 => LogLevel.Warning,  // > 5s
            > 2000 => LogLevel.Information,  // > 2s
            > 1000 => LogLevel.Debug,    // > 1s
            _ => LogLevel.Trace           // <= 1s
        };
    }
}