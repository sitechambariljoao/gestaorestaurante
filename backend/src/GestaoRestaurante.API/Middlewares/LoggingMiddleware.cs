using System.Diagnostics;
using System.Text;

namespace GestaoRestaurante.API.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        // Log da requisição
        await LogRequestAsync(context, requestId);

        // Capturar response original
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyMemoryStream = new MemoryStream();
        context.Response.Body = responseBodyMemoryStream;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log da resposta
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

            // Restaurar response body original
            responseBodyMemoryStream.Seek(0, SeekOrigin.Begin);
            await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;

        var requestLog = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            Headers = GetHeaders(request.Headers),
            UserAgent = request.Headers.UserAgent.ToString(),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Requisição recebida: {@RequestLog}", requestLog);

        // Log do body apenas para métodos que podem ter body e não são muito grandes
        if (ShouldLogRequestBody(request))
        {
            request.EnableBuffering();
            var body = await ReadBodyAsync(request.Body);
            request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                _logger.LogDebug("Request Body [{RequestId}]: {Body}", requestId, body);
            }
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
    {
        var response = context.Response;

        var responseLog = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength,
            ElapsedMs = elapsedMs,
            Headers = GetHeaders(response.Headers),
            Timestamp = DateTime.UtcNow
        };

        var logLevel = GetLogLevel(response.StatusCode);
        _logger.Log(logLevel, "Resposta enviada: {@ResponseLog}", responseLog);

        // Log do response body apenas em desenvolvimento e para erros
        if (ShouldLogResponseBody(response))
        {
            var body = await ReadResponseBodyAsync(context.Response.Body);
            if (!string.IsNullOrEmpty(body))
            {
                _logger.LogDebug("Response Body [{RequestId}]: {Body}", requestId, body);
            }
        }
    }

    private static Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
    {
        var headerDict = new Dictionary<string, string>();
        var headersToLog = new[] { "Content-Type", "Accept", "Authorization", "User-Agent", "Referer" };

        foreach (var header in headers.Where(h => headersToLog.Contains(h.Key)))
        {
            // Mascarar Authorization header
            if (header.Key == "Authorization")
            {
                headerDict[header.Key] = "Bearer ***";
            }
            else
            {
                headerDict[header.Key] = header.Value.ToString();
            }
        }

        return headerDict;
    }

    private static bool ShouldLogRequestBody(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength > 10000) // 10KB limite
            return false;

        var method = request.Method.ToUpper();
        if (method != "POST" && method != "PUT" && method != "PATCH")
            return false;

        var contentType = request.ContentType?.ToLower();
        return contentType != null && 
               (contentType.Contains("application/json") || 
                contentType.Contains("application/xml") ||
                contentType.Contains("text/"));
    }

    private static bool ShouldLogResponseBody(HttpResponse response)
    {
        if (response.ContentLength == null || response.ContentLength > 10000) // 10KB limite
            return false;

        // Log apenas para erros (4xx, 5xx) ou em desenvolvimento
        return response.StatusCode >= 400;
    }

    private static async Task<string> ReadBodyAsync(Stream body)
    {
        body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        body.Seek(0, SeekOrigin.Begin);
        return content;
    }

    private static async Task<string> ReadResponseBodyAsync(Stream body)
    {
        body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        body.Seek(0, SeekOrigin.Begin);
        return content;
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}