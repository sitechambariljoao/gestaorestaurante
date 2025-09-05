using GestaoRestaurante.API.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GestaoRestaurante.API.Extensions;

/// <summary>
/// Extensões para configuração de health checks avançados
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adiciona todos os health checks customizados
    /// </summary>
    public static IServiceCollection AddAdvancedHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Health checks customizados
            .AddCheck<DatabaseHealthCheck>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready", "database" })
            
            .AddCheck<CacheHealthCheck>(
                name: "cache",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "ready", "cache" })
            
            .AddCheck<SystemResourcesHealthCheck>(
                name: "system_resources",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "live", "system" })
            
            .AddCheck<ExternalServicesHealthCheck>(
                name: "external_services",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "ready", "external" })
            
            // Health checks built-in
            .AddCheck<DatabaseHealthCheck>(
                name: "ef_database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready", "database", "ef" });

        // Registrar dependências dos health checks
        services.AddScoped<DatabaseHealthCheck>();
        services.AddScoped<CacheHealthCheck>();
        services.AddScoped<SystemResourcesHealthCheck>();
        services.AddScoped<ExternalServicesHealthCheck>();
        
        // HttpClient para health checks de serviços externos
        services.AddHttpClient<ExternalServicesHealthCheck>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "GestaoRestaurante-HealthCheck");
        });

        return services;
    }

    /// <summary>
    /// Configura os endpoints de health check
    /// </summary>
    public static IApplicationBuilder UseAdvancedHealthChecks(this IApplicationBuilder app)
    {
        // Health check simples para load balancers
        app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false // Não executar checks, apenas retornar 200 OK
        });

        // Health check "liveness" - verifica se a aplicação está rodando
        app.UseHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Health check "readiness" - verifica se a aplicação está pronta para receber tráfego
        app.UseHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Health check detalhado com todas as informações
        app.UseHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true, // Todos os checks
            ResponseWriter = WriteDetailedHealthCheckResponse
        });

        return app;
    }

    /// <summary>
    /// Escreve resposta de health check simples
    /// </summary>
    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                tags = entry.Value.Tags
            })
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Escreve resposta de health check detalhada
    /// </summary>
    private static async Task WriteDetailedHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow,
            environment = new
            {
                machineName = Environment.MachineName,
                osVersion = Environment.OSVersion.ToString(),
                processId = Environment.ProcessId,
                workingDirectory = Environment.CurrentDirectory
            },
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                exception = entry.Value.Exception?.Message,
                tags = entry.Value.Tags,
                data = entry.Value.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
            })
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
}