using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoRestaurante.Application.Common.Monitoring;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.API.Authorization;
using System.Diagnostics;

namespace GestaoRestaurante.API.Controllers;

/// <summary>
/// Controller para exposição de métricas e monitoramento
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ModuleAuthorization("ADMIN")] // Apenas administradores podem ver métricas
public class MetricsController : ControllerBase
{
    private readonly GestaoRestaurante.Application.Common.Monitoring.IApplicationMetrics _metrics;
    private readonly IPerformanceProfiler _profiler;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(
        GestaoRestaurante.Application.Common.Monitoring.IApplicationMetrics metrics, 
        IPerformanceProfiler profiler,
        ILogger<MetricsController> logger)
    {
        _metrics = metrics;
        _profiler = profiler;
        _logger = logger;
    }

    /// <summary>
    /// Obtém sumário completo de métricas da aplicação
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<MetricsSummary>> GetMetricsSummary()
    {
        try
        {
            using var timing = _metrics.Time("metrics.get_summary");
            
            var summary = await _metrics.GetMetricsSummaryAsync();
            
            _logger.LogDebug("Métricas consultadas: {CounterCount} contadores, {GaugeCount} gauges, {HistogramCount} histogramas",
                summary.Counters.Count, summary.Gauges.Count, summary.Histograms.Count);
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter sumário de métricas");
            return StatusCode(500, "Erro interno ao consultar métricas");
        }
    }

    /// <summary>
    /// Obtém estatísticas de performance
    /// </summary>
    [HttpGet("performance")]
    public async Task<ActionResult<PerformanceStats>> GetPerformanceStats()
    {
        try
        {
            using var timing = _metrics.Time("metrics.get_performance");
            
            var stats = await _profiler.GetStatsAsync();
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas de performance");
            return StatusCode(500, "Erro interno ao consultar performance");
        }
    }

    /// <summary>
    /// Obtém métricas do sistema operacional
    /// </summary>
    [HttpGet("system")]
    public ActionResult<SystemMetrics> GetSystemMetrics()
    {
        try
        {
            using var timing = _metrics.Time("metrics.get_system");
            
            var process = Process.GetCurrentProcess();
            
            var systemMetrics = new SystemMetrics
            {
                ProcessId = process.Id,
                StartTime = process.StartTime,
                Uptime = DateTime.Now - process.StartTime,
                WorkingSetMB = process.WorkingSet64 / 1024 / 1024,
                PrivateMemoryMB = process.PrivateMemorySize64 / 1024 / 1024,
                VirtualMemoryMB = process.VirtualMemorySize64 / 1024 / 1024,
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount,
                GcGen0Collections = GC.CollectionCount(0),
                GcGen1Collections = GC.CollectionCount(1),
                GcGen2Collections = GC.CollectionCount(2),
                GcTotalMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024,
                ProcessorCount = Environment.ProcessorCount,
                OSVersion = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName
            };

            return Ok(systemMetrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter métricas do sistema");
            return StatusCode(500, "Erro interno ao consultar métricas do sistema");
        }
    }

    /// <summary>
    /// Obtém métricas específicas por nome ou padrão
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<QueryMetricsResponse>> QueryMetrics(
        [FromQuery] string? pattern = null,
        [FromQuery] string? type = null)
    {
        try
        {
            using var timing = _metrics.Time("metrics.query", new Dictionary<string, string> 
            { 
                ["pattern"] = pattern ?? "all",
                ["type"] = type ?? "all"
            });
            
            var summary = await _metrics.GetMetricsSummaryAsync();
            var response = new QueryMetricsResponse();

            // Filtrar por padrão se especificado
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                
                response.Counters = summary.Counters.Where(c => regex.IsMatch(c.Name)).ToList();
                response.Gauges = summary.Gauges.Where(g => regex.IsMatch(g.Name)).ToList();
                response.Histograms = summary.Histograms.Where(h => regex.IsMatch(h.Name)).ToList();
            }
            else
            {
                response.Counters = summary.Counters;
                response.Gauges = summary.Gauges;
                response.Histograms = summary.Histograms;
            }

            // Filtrar por tipo se especificado
            if (!string.IsNullOrWhiteSpace(type))
            {
                switch (type.ToLowerInvariant())
                {
                    case "counter":
                        response.Gauges.Clear();
                        response.Histograms.Clear();
                        break;
                    case "gauge":
                        response.Counters.Clear();
                        response.Histograms.Clear();
                        break;
                    case "histogram":
                        response.Counters.Clear();
                        response.Gauges.Clear();
                        break;
                }
            }

            response.TotalMetrics = response.Counters.Count + response.Gauges.Count + response.Histograms.Count;
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar métricas com padrão '{Pattern}' e tipo '{Type}'", pattern, type);
            return StatusCode(500, "Erro interno ao consultar métricas");
        }
    }

    /// <summary>
    /// Reseta todas as métricas (use com cuidado)
    /// </summary>
    [HttpPost("reset")]
    public ActionResult ResetMetrics()
    {
        try
        {
            _metrics.Reset();
            
            _logger.LogWarning("Métricas foram resetadas pelo usuário {UserId}", User.Identity?.Name);
            
            return Ok(new { message = "Métricas resetadas com sucesso", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resetar métricas");
            return StatusCode(500, "Erro interno ao resetar métricas");
        }
    }

    /// <summary>
    /// Grava uma métrica customizada (para debugging/testing)
    /// </summary>
    [HttpPost("custom")]
    public ActionResult RecordCustomMetric([FromBody] CustomMetricRequest request)
    {
        try
        {
            switch (request.Type.ToLowerInvariant())
            {
                case "counter":
                    _metrics.IncrementCounter(request.Name, request.Tags);
                    break;
                
                case "gauge":
                    if (request.Value.HasValue)
                    {
                        _metrics.RecordGauge(request.Name, request.Value.Value, request.Tags);
                    }
                    else
                    {
                        return BadRequest("Valor é obrigatório para gauge");
                    }
                    break;
                
                case "histogram":
                    if (request.Value.HasValue)
                    {
                        _metrics.RecordHistogram(request.Name, request.Value.Value, request.Tags);
                    }
                    else
                    {
                        return BadRequest("Valor é obrigatório para histogram");
                    }
                    break;
                
                default:
                    return BadRequest($"Tipo de métrica inválido: {request.Type}");
            }

            _logger.LogDebug("Métrica customizada gravada: {Type} {Name} = {Value}", 
                request.Type, request.Name, request.Value);
            
            return Ok(new { message = "Métrica gravada com sucesso", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gravar métrica customizada");
            return StatusCode(500, "Erro interno ao gravar métrica");
        }
    }
}

/// <summary>
/// Métricas do sistema
/// </summary>
public class SystemMetrics
{
    public int ProcessId { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Uptime { get; set; }
    public long WorkingSetMB { get; set; }
    public long PrivateMemoryMB { get; set; }
    public long VirtualMemoryMB { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public int GcGen0Collections { get; set; }
    public int GcGen1Collections { get; set; }
    public int GcGen2Collections { get; set; }
    public long GcTotalMemoryMB { get; set; }
    public int ProcessorCount { get; set; }
    public string OSVersion { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
}

/// <summary>
/// Resposta para consulta de métricas
/// </summary>
public class QueryMetricsResponse
{
    public List<CounterSnapshot> Counters { get; set; } = new();
    public List<GaugeSnapshot> Gauges { get; set; } = new();
    public List<HistogramSnapshot> Histograms { get; set; } = new();
    public int TotalMetrics { get; set; }
}

/// <summary>
/// Request para métrica customizada
/// </summary>
public class CustomMetricRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // counter, gauge, histogram
    public double? Value { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}