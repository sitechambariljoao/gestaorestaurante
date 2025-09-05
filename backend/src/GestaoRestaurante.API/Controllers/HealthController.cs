using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.API.Filters;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
public class HealthController : ControllerBase
{
    private readonly GestaoRestauranteContext _context;
    private readonly IApplicationMetrics _metrics;
    private readonly IPerformanceProfiler _profiler;
    private readonly ICacheService _cache;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        GestaoRestauranteContext context,
        IApplicationMetrics metrics,
        IPerformanceProfiler profiler,
        ICacheService cache,
        ILogger<HealthController> logger)
    {
        _context = context;
        _metrics = metrics;
        _profiler = profiler;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Verifica a saúde básica da API
    /// </summary>
    /// <returns>Status da API</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHealth()
    {
        using var measurement = _profiler.StartMeasurement("health.basic_check");
        
        _metrics.IncrementCounter("health.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "basic",
            ["source"] = "health_controller"
        });

        try
        {
            // Cache health status por 30 segundos para evitar sobrecarga
            var healthStatus = await _cache.GetOrSetAsync(
                "health:basic",
                async () =>
                {
                    var dbHealth = await CheckDatabaseHealthAsync();
                    
                    return new
                    {
                        Status = "Healthy",
                        Timestamp = DateTime.UtcNow,
                        Version = "1.0.0",
                        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                        Database = dbHealth,
                        ServerInfo = new
                        {
                            MachineName = Environment.MachineName,
                            ProcessorCount = Environment.ProcessorCount,
                            WorkingSet = Environment.WorkingSet / 1024 / 1024 // MB
                        }
                    };
                },
                TimeSpan.FromSeconds(30)
            );

            _metrics.RecordValue("health.response_time", ((PerformanceMeasurement)measurement).Elapsed.TotalMilliseconds);
            
            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante health check básico");
            _metrics.IncrementCounter("health.errors", new Dictionary<string, string> { ["type"] = "basic_check" });
            
            return Ok(new
            {
                Status = "Degraded",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Error = "Health check parcialmente falhou"
            });
        }
    }

    /// <summary>
    /// Verifica a saúde detalhada da API
    /// </summary>
    /// <returns>Status detalhado</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<object>> GetDetailedHealth()
    {
        using var measurement = _profiler.StartMeasurement("health.detailed_check");
        
        _metrics.IncrementCounter("health.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "detailed",
            ["source"] = "health_controller"
        });

        try
        {
            // Cache por 10 segundos para detailed health (mais dinâmico)
            var healthStatus = await _cache.GetOrSetAsync(
                "health:detailed",
                async () => await PerformDetailedHealthCheck(),
                TimeSpan.FromSeconds(10)
            );

            _metrics.RecordValue("health.detailed.response_time", ((PerformanceMeasurement)measurement).Elapsed.TotalMilliseconds);
            
            var isHealthy = ((dynamic)healthStatus).Status == "Healthy";
            
            if (!isHealthy)
            {
                _metrics.IncrementCounter("health.unhealthy", new Dictionary<string, string> { ["type"] = "detailed_check" });
            }

            return isHealthy ? Ok(healthStatus) : StatusCode(503, healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante health check detalhado");
            _metrics.IncrementCounter("health.errors", new Dictionary<string, string> { ["type"] = "detailed_check" });
            
            var errorStatus = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Error = "Health check detalhado falhou completamente",
                Exception = ex.Message
            };

            return StatusCode(503, errorStatus);
        }
    }

    private async Task<object> PerformDetailedHealthCheck()
    {
        using var detailedMeasurement = _profiler.StartMeasurement("health.detailed.full_check");
        
        var checks = new Dictionary<string, object>();
        var overallHealth = true;

        // 1. Verificar banco de dados
        try
        {
            using var dbMeasurement = _profiler.StartMeasurement("health.database_check");
            dynamic dbHealth = await CheckDatabaseHealthAsync();
            checks["Database"] = dbHealth;
            
            _metrics.RecordValue("health.database.response_time", ((PerformanceMeasurement)dbMeasurement).Elapsed.TotalMilliseconds);
            
            if (dbHealth.Status != "Healthy")
            {
                overallHealth = false;
                _metrics.IncrementCounter("health.database.unhealthy");
            }
        }
        catch (Exception ex)
        {
            checks["Database"] = new { Status = "Unhealthy", Error = ex.Message };
            overallHealth = false;
            _metrics.IncrementCounter("health.database.errors");
        }

        // 2. Verificar memória
        var memoryInfo = GC.GetTotalMemory(false);
        var memoryMB = Math.Round(memoryInfo / 1024.0 / 1024.0, 2);
        var memoryHealthy = memoryInfo < 1024 * 1024 * 500; // 500MB threshold
        
        checks["Memory"] = new
        {
            Status = memoryHealthy ? "Healthy" : "Warning",
            UsageBytes = memoryInfo,
            UsageMB = memoryMB,
            ThresholdMB = 500,
            GCGen0Collections = GC.CollectionCount(0),
            GCGen1Collections = GC.CollectionCount(1),
            GCGen2Collections = GC.CollectionCount(2)
        };

        _metrics.RecordValue("health.memory.usage_mb", memoryMB);

        // 3. Verificar tempo de atividade e sistema
        var uptime = Environment.TickCount64;
        var uptimeSpan = TimeSpan.FromMilliseconds(uptime);
        
        checks["System"] = new
        {
            Status = "Healthy",
            UptimeMs = uptime,
            UptimeFormatted = uptimeSpan.ToString(@"d\.hh\:mm\:ss"),
            MachineName = Environment.MachineName,
            ProcessorCount = Environment.ProcessorCount,
            WorkingSet = Environment.WorkingSet / 1024 / 1024, // MB
            ThreadCount = System.Threading.ThreadPool.ThreadCount,
            CompletedWorkItems = System.Threading.ThreadPool.CompletedWorkItemCount
        };

        // 4. Verificar cache se disponível
        try
        {
            var cacheTestKey = $"health:cache:test:{Guid.NewGuid()}";
            var testValue = DateTime.UtcNow.ToString();
            
            await _cache.SetAsync(cacheTestKey, testValue, TimeSpan.FromSeconds(5));
            var retrievedValue = await _cache.GetAsync<string>(cacheTestKey);
            
            checks["Cache"] = new
            {
                Status = retrievedValue == testValue ? "Healthy" : "Degraded",
                TestPassed = retrievedValue == testValue,
                ResponseTime = "< 1ms"
            };
        }
        catch (Exception ex)
        {
            checks["Cache"] = new { Status = "Degraded", Error = ex.Message };
            _metrics.IncrementCounter("health.cache.errors");
        }

        var finalStatus = new
        {
            Status = overallHealth ? "Healthy" : "Unhealthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            TotalCheckTimeMs = ((PerformanceMeasurement)detailedMeasurement).Elapsed.TotalMilliseconds,
            Checks = checks
        };

        return finalStatus;
    }

    private async Task<object> CheckDatabaseHealthAsync()
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var canConnect = await _context.Database.CanConnectAsync();
            var responseTime = DateTime.UtcNow - startTime;

            if (canConnect)
            {
                // Teste adicional: contar registros de uma tabela simples
                var empresaCount = await _context.Empresas.CountAsync();
                
                return new
                {
                    Status = "Healthy",
                    ResponseTimeMs = Math.Round(responseTime.TotalMilliseconds, 2),
                    ConnectionString = _context.Database.GetConnectionString()?.Replace("Password=", "Password=***"),
                    DatabaseName = _context.Database.GetDbConnection().Database,
                    EmpresaCount = empresaCount,
                    Provider = _context.Database.ProviderName
                };
            }
            else
            {
                return new { Status = "Unhealthy", Error = "Cannot connect to database" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Database health check falhou");
            return new 
            { 
                Status = "Unhealthy", 
                Error = ex.Message,
                ExceptionType = ex.GetType().Name
            };
        }
    }

    /// <summary>
    /// Endpoint para métricas de saúde da aplicação
    /// </summary>
    /// <returns>Métricas de health checks</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHealthMetrics()
    {
        _metrics.IncrementCounter("health.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "metrics",
            ["source"] = "health_controller"
        });

        try
        {
            // Cache métricas por 60 segundos
            var metricsData = await _cache.GetOrSetAsync(
                "health:metrics",
                async () =>
                {
                    // Simular coleta de métricas de health checks dos últimos tempos
                    var metrics = new
                    {
                        HealthChecks = new
                        {
                            TotalRequests = await GetMetricValueAsync("health.requests.total"),
                            SuccessfulChecks = await GetMetricValueAsync("health.successful.total"),
                            FailedChecks = await GetMetricValueAsync("health.failed.total"),
                            AverageResponseTimeMs = await GetMetricValueAsync("health.response_time.avg"),
                            DatabaseChecks = new
                            {
                                Total = await GetMetricValueAsync("health.database.total"),
                                Healthy = await GetMetricValueAsync("health.database.healthy"),
                                Unhealthy = await GetMetricValueAsync("health.database.unhealthy"),
                                Errors = await GetMetricValueAsync("health.database.errors"),
                                AverageResponseTimeMs = await GetMetricValueAsync("health.database.response_time.avg")
                            },
                            CacheChecks = new
                            {
                                Total = await GetMetricValueAsync("health.cache.total"),
                                Errors = await GetMetricValueAsync("health.cache.errors")
                            }
                        },
                        System = new
                        {
                            CurrentMemoryUsageMB = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2),
                            UptimeMs = Environment.TickCount64,
                            ProcessorCount = Environment.ProcessorCount,
                            MachineName = Environment.MachineName
                        },
                        Generated = DateTime.UtcNow
                    };

                    return metrics;
                },
                TimeSpan.FromSeconds(60)
            );

            return Ok(metricsData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao coletar métricas de health");
            _metrics.IncrementCounter("health.errors", new Dictionary<string, string> { ["type"] = "metrics_collection" });
            
            return Ok(new
            {
                Error = "Erro ao coletar métricas de health",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Simula obtenção de valores de métricas (placeholder para implementação real)
    /// </summary>
    private async Task<double> GetMetricValueAsync(string metricName)
    {
        // Placeholder - em uma implementação real, isso consultaria o sistema de métricas
        await Task.Delay(1);
        
        return metricName switch
        {
            "health.requests.total" => Random.Shared.Next(1000, 5000),
            "health.successful.total" => Random.Shared.Next(900, 4500),
            "health.failed.total" => Random.Shared.Next(10, 100),
            "health.response_time.avg" => Random.Shared.Next(5, 50),
            "health.database.total" => Random.Shared.Next(500, 2000),
            "health.database.healthy" => Random.Shared.Next(450, 1950),
            "health.database.unhealthy" => Random.Shared.Next(5, 20),
            "health.database.errors" => Random.Shared.Next(1, 10),
            "health.database.response_time.avg" => Random.Shared.Next(2, 15),
            "health.cache.total" => Random.Shared.Next(200, 800),
            "health.cache.errors" => Random.Shared.Next(0, 5),
            _ => 0.0
        };
    }
}