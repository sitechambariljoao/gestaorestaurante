using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace GestaoRestaurante.API.Filters;

/// <summary>
/// Action Filter que monitora performance e adiciona headers de timing
/// </summary>
public class PerformanceActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items["PerformanceTimer"] = stopwatch;
        context.HttpContext.Items["StartTime"] = DateTime.UtcNow;
        
        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        if (context.HttpContext.Items["PerformanceTimer"] is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var startTime = (DateTime)context.HttpContext.Items["StartTime"]!;
            
            // Adicionar headers de performance
            context.HttpContext.Response.Headers.Add("X-Response-Time-Ms", elapsedMs.ToString());
            context.HttpContext.Response.Headers.Add("X-Start-Time", startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            context.HttpContext.Response.Headers.Add("X-End-Time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            
            // Categorizar performance
            var category = elapsedMs switch
            {
                < 100 => "Fast",
                < 500 => "Normal", 
                < 1000 => "Slow",
                _ => "Critical"
            };
            
            context.HttpContext.Response.Headers.Add("X-Performance-Category", category);
            
            // Log para requests lentos
            if (elapsedMs > 1000)
            {
                var actionName = context.ActionDescriptor.DisplayName;
                Console.WriteLine($"[SLOW REQUEST] {actionName} took {elapsedMs}ms - Category: {category}");
            }
        }
    }

}