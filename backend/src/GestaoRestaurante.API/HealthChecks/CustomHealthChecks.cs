using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Application.Common.Caching;

namespace GestaoRestaurante.API.HealthChecks;

/// <summary>
/// Health check customizado para banco de dados com verificações detalhadas
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly GestaoRestauranteContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(GestaoRestauranteContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            // Verificar conexão básica
            await _context.Database.CanConnectAsync(cancellationToken);

            // Verificar se há dados básicos (pelo menos 1 empresa)
            var empresaCount = await _context.Empresas.CountAsync(cancellationToken);
            
            // Verificar performance da query
            var queryTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            var data = new Dictionary<string, object>
            {
                ["connection_time_ms"] = queryTime,
                ["empresas_count"] = empresaCount,
                ["database_name"] = _context.Database.GetDbConnection().Database
            };

            if (queryTime > 1000) // > 1 segundo
            {
                _logger.LogWarning("Database health check slow: {QueryTime}ms", queryTime);
                return HealthCheckResult.Degraded($"Database responding slowly ({queryTime}ms)", data: data);
            }

            if (empresaCount == 0)
            {
                _logger.LogWarning("No empresas found in database - system may not be seeded");
                return HealthCheckResult.Degraded("No data found - system may not be initialized", data: data);
            }

            return HealthCheckResult.Healthy($"Database operational ({queryTime}ms)", data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}

/// <summary>
/// Health check para sistema de cache
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheHealthCheck> _logger;

    public CacheHealthCheck(ICacheService cacheService, ILogger<CacheHealthCheck> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var testKey = $"health_check_{Guid.NewGuid()}";
            var testValue = "test_value";
            var stopwatch = Stopwatch.StartNew();

            // Testar write
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromMinutes(1));

            // Testar read
            var retrievedValue = await _cacheService.GetAsync<string>(testKey);

            // Cleanup
            await _cacheService.RemoveAsync(testKey);

            stopwatch.Stop();
            var operationTime = stopwatch.ElapsedMilliseconds;

            var data = new Dictionary<string, object>
            {
                ["operation_time_ms"] = operationTime,
                ["write_success"] = true,
                ["read_success"] = retrievedValue == testValue
            };

            if (retrievedValue != testValue)
            {
                return HealthCheckResult.Unhealthy("Cache read/write test failed", data: data);
            }

            if (operationTime > 100) // > 100ms
            {
                _logger.LogWarning("Cache operations slow: {OperationTime}ms", operationTime);
                return HealthCheckResult.Degraded($"Cache responding slowly ({operationTime}ms)", data: data);
            }

            return HealthCheckResult.Healthy($"Cache operational ({operationTime}ms)", data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache health check failed");
            return HealthCheckResult.Unhealthy("Cache system failed", ex);
        }
    }
}

/// <summary>
/// Health check para métricas de sistema
/// </summary>
public class SystemResourcesHealthCheck : IHealthCheck
{
    private readonly ILogger<SystemResourcesHealthCheck> _logger;

    public SystemResourcesHealthCheck(ILogger<SystemResourcesHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetCurrentProcess();
            
            // Memória
            var workingSetMB = process.WorkingSet64 / 1024 / 1024;
            var privateMemoryMB = process.PrivateMemorySize64 / 1024 / 1024;
            
            // CPU (estimativa baseada no tempo de processamento)
            var totalProcessorTime = process.TotalProcessorTime;
            var cpuUsagePercent = (totalProcessorTime.TotalMilliseconds / Environment.TickCount) * 100;

            // Threads
            var threadCount = process.Threads.Count;

            // GC
            var gen0Collections = GC.CollectionCount(0);
            var gen1Collections = GC.CollectionCount(1);
            var gen2Collections = GC.CollectionCount(2);
            var totalMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                ["working_set_mb"] = workingSetMB,
                ["private_memory_mb"] = privateMemoryMB,
                ["cpu_usage_percent"] = Math.Round(cpuUsagePercent, 2),
                ["thread_count"] = threadCount,
                ["gc_gen0_collections"] = gen0Collections,
                ["gc_gen1_collections"] = gen1Collections,
                ["gc_gen2_collections"] = gen2Collections,
                ["gc_total_memory_mb"] = totalMemoryMB,
                ["uptime_minutes"] = Math.Round(TimeSpan.FromMilliseconds(Environment.TickCount).TotalMinutes, 1)
            };

            var warnings = new List<string>();

            // Verificar limites críticos
            if (workingSetMB > 1000) // > 1GB
            {
                warnings.Add($"High memory usage: {workingSetMB}MB");
            }

            if (threadCount > 100)
            {
                warnings.Add($"High thread count: {threadCount}");
            }

            if (gen2Collections > 10 && Environment.TickCount > 60000) // Muitas coletas Gen2 após 1 minuto
            {
                warnings.Add($"Frequent Gen2 collections: {gen2Collections}");
            }

            if (warnings.Any())
            {
                var warningMessage = string.Join("; ", warnings);
                _logger.LogWarning("System resources warnings: {Warnings}", warningMessage);
                return Task.FromResult(HealthCheckResult.Degraded(warningMessage, data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy("System resources normal", data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System resources health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Could not check system resources", ex));
        }
    }
}

/// <summary>
/// Health check para serviços externos (futura integração)
/// </summary>
public class ExternalServicesHealthCheck : IHealthCheck
{
    private readonly ILogger<ExternalServicesHealthCheck> _logger;
    private readonly HttpClient _httpClient;

    public ExternalServicesHealthCheck(ILogger<ExternalServicesHealthCheck> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, object>();
        var issues = new List<string>();

        try
        {
            // Verificar serviço de CEP (exemplo)
            var cepResult = await CheckCepService(cancellationToken);
            results["cep_service"] = cepResult.IsHealthy;
            if (!cepResult.IsHealthy)
            {
                issues.Add("CEP service unavailable");
            }

            // Adicionar outros serviços conforme necessário
            // var emailResult = await CheckEmailService(cancellationToken);
            // results["email_service"] = emailResult.IsHealthy;

            if (issues.Any())
            {
                var message = string.Join("; ", issues);
                return HealthCheckResult.Degraded(message, data: results);
            }

            return HealthCheckResult.Healthy("All external services operational", data: results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External services health check failed");
            return HealthCheckResult.Unhealthy("External services check failed", ex, results);
        }
    }

    private async Task<(bool IsHealthy, TimeSpan ResponseTime)> CheckCepService(CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Testar com um CEP conhecido
            using var response = await _httpClient.GetAsync("https://viacep.com.br/ws/01310-100/json/", cancellationToken);
            
            stopwatch.Stop();
            
            return (response.IsSuccessStatusCode, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CEP service check failed");
            return (false, TimeSpan.Zero);
        }
    }
}