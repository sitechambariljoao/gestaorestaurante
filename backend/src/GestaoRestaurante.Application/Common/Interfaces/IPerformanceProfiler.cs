namespace GestaoRestaurante.Application.Common.Interfaces;

/// <summary>
/// Interface para profiling de performance da aplicação
/// </summary>
public interface IPerformanceProfiler
{
    /// <summary>
    /// Inicia uma medição de performance
    /// </summary>
    IDisposable StartMeasurement(string operationName);

    /// <summary>
    /// Registra uma métrica personalizada
    /// </summary>
    void RecordMetric(string name, double value, string unit = "ms");

    /// <summary>
    /// Obtém estatísticas de performance
    /// </summary>
    Task<PerformanceStats> GetStatsAsync();
}

/// <summary>
/// Estatísticas de performance
/// </summary>
public class PerformanceStats
{
    public DateTime LastReset { get; set; }
    public TimeSpan Uptime { get; set; }
    public Dictionary<string, double> Metrics { get; set; } = new();
    public Dictionary<string, OperationStats> Operations { get; set; } = new();
}

/// <summary>
/// Estatísticas de uma operação específica
/// </summary>
public class OperationStats
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public double AverageMs { get; set; }
    public double MinMs { get; set; }
    public double MaxMs { get; set; }
    public double TotalMs { get; set; }
    public DateTime LastExecution { get; set; }
}

/// <summary>
/// Implementação para medição de performance
/// </summary>
public class PerformanceMeasurement : IDisposable
{
    private readonly string _operationName;
    private readonly Action<TimeSpan> _onCompleted;
    private readonly DateTime _startTime;
    private bool _disposed;

    public TimeSpan Elapsed => DateTime.UtcNow - _startTime;

    public PerformanceMeasurement(string operationName, Action<TimeSpan> onCompleted)
    {
        _operationName = operationName;
        _onCompleted = onCompleted;
        _startTime = DateTime.UtcNow;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            var elapsed = DateTime.UtcNow - _startTime;
            _onCompleted(elapsed);
            _disposed = true;
        }
    }
}