using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;

namespace GestaoRestaurante.API.Filters;

/// <summary>
/// Action Filter que registra logs detalhados de requests e responses
/// </summary>
public class LoggingActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items["ActionTimer"] = stopwatch;

        var request = context.HttpContext.Request;
        var actionName = context.ActionDescriptor.DisplayName;
        var correlationId = context.HttpContext.TraceIdentifier;

        var logData = new
        {
            CorrelationId = correlationId,
            Action = actionName,
            Method = request.Method,
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            Timestamp = DateTime.UtcNow,
            Arguments = FilterSensitiveData(context.ActionArguments)
        };

        Console.WriteLine($"[REQUEST] {JsonSerializer.Serialize(logData, new JsonSerializerOptions { WriteIndented = false })}");
        
        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        if (context.HttpContext.Items["ActionTimer"] is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var correlationId = context.HttpContext.TraceIdentifier;
            var actionName = context.ActionDescriptor.DisplayName;

            var logData = new
            {
                CorrelationId = correlationId,
                Action = actionName,
                StatusCode = context.HttpContext.Response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow,
                HasException = context.Exception != null,
                ExceptionMessage = context.Exception?.Message
            };

            var logLevel = context.Exception != null ? "ERROR" : 
                          stopwatch.ElapsedMilliseconds > 1000 ? "WARN" : "INFO";

            Console.WriteLine($"[RESPONSE-{logLevel}] {JsonSerializer.Serialize(logData, new JsonSerializerOptions { WriteIndented = false })}");
        }
    }

    private static Dictionary<string, object?> FilterSensitiveData(IDictionary<string, object?> arguments)
    {
        var filtered = new Dictionary<string, object?>();
        var sensitiveKeys = new[] { "password", "senha", "token", "secret", "key" };

        foreach (var arg in arguments)
        {
            if (sensitiveKeys.Any(key => arg.Key.Contains(key, StringComparison.OrdinalIgnoreCase)))
            {
                filtered[arg.Key] = "***FILTERED***";
            }
            else
            {
                filtered[arg.Key] = arg.Value;
            }
        }

        return filtered;
    }
}