using GestaoRestaurante.API.Middlewares;
using GestaoRestaurante.API.Middleware;

namespace GestaoRestaurante.API.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adiciona todos os middlewares customizados do sistema na ordem correta
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder para chaining</returns>
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        // 1. Correlation ID - deve ser o primeiro para rastrear toda a requisição
        app.UseMiddleware<CorrelationIdMiddleware>();

        // 2. Response Time - medir tempo total incluindo outros middlewares
        app.UseMiddleware<ResponseTimeMiddleware>();

        // 3. Global Exception Handling - capturar todas as exceções
        app.UseGlobalExceptionHandler();

        // 4. Logging - logar requisições/respostas (após exception handler)
        app.UseMiddleware<LoggingMiddleware>();

        return app;
    }

    /// <summary>
    /// Configura o logging estruturado para incluir informações contextuais
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection para chaining</returns>
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            
            // Configurar níveis de log
            builder.SetMinimumLevel(LogLevel.Information);
            
            // Filtros específicos
            builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
            builder.AddFilter("System.Net.Http", LogLevel.Warning);
        });

        return services;
    }
}