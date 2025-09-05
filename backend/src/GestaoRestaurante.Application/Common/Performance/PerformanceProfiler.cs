using System.Collections.Concurrent;
using GestaoRestaurante.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestaoRestaurante.Application.Common.Performance;

/// <summary>
/// Implementação do profiler de performance com thread-safety
/// </summary>
public class PerformanceProfiler : IPerformanceProfiler
{
    private readonly ILogger<PerformanceProfiler> _logger;
    private readonly ConcurrentDictionary<string, List<double>> _operationTimes;
    private readonly ConcurrentDictionary<string, double> _metrics;
    private readonly DateTime _startTime;

    public PerformanceProfiler(ILogger<PerformanceProfiler> logger)
    {
        _logger = logger;
        _operationTimes = new ConcurrentDictionary<string, List<double>>();
        _metrics = new ConcurrentDictionary<string, double>();
        _startTime = DateTime.UtcNow;
    }

    public IDisposable StartMeasurement(string operationName)
    {
        return new PerformanceMeasurement(operationName, elapsed =>
        {
            var milliseconds = elapsed.TotalMilliseconds;
            RecordOperationTime(operationName, milliseconds);
            
            // Log operações lentas
            if (milliseconds > 1000) // > 1 segundo
            {
                _logger.LogWarning(
                    "Operação lenta detectada: {Operation} - {Duration}ms", 
                    operationName, 
                    milliseconds);
            }
        });
    }

    public void RecordMetric(string name, double value, string unit = "ms")
    {
        _metrics.AddOrUpdate(name, value, (key, oldValue) => value);
        
        _logger.LogDebug(
            "Métrica registrada: {MetricName} = {Value} {Unit}", 
            name, value, unit);
    }

    public Task<PerformanceStats> GetStatsAsync()
    {
        var stats = new PerformanceStats
        {
            LastReset = _startTime,
            Uptime = DateTime.UtcNow - _startTime,
            Metrics = new Dictionary<string, double>(_metrics)
        };

        foreach (var operation in _operationTimes)
        {
            var times = operation.Value.ToArray(); // Thread-safe copy
            if (times.Length > 0)
            {
                stats.Operations[operation.Key] = new OperationStats
                {
                    Name = operation.Key,
                    Count = times.Length,
                    AverageMs = times.Average(),
                    MinMs = times.Min(),
                    MaxMs = times.Max(),
                    TotalMs = times.Sum(),
                    LastExecution = DateTime.UtcNow
                };
            }
        }

        return Task.FromResult(stats);
    }

    private void RecordOperationTime(string operationName, double milliseconds)
    {
        _operationTimes.AddOrUpdate(
            operationName,
            new List<double> { milliseconds },
            (key, existingList) =>
            {
                lock (existingList)
                {
                    existingList.Add(milliseconds);
                    
                    // Manter apenas últimas 1000 medições para evitar memory leak
                    if (existingList.Count > 1000)
                    {
                        existingList.RemoveRange(0, 100); // Remove as 100 mais antigas
                    }
                }
                return existingList;
            });
    }
}