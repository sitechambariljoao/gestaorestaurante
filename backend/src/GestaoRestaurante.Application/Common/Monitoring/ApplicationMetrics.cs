using System.Collections.Concurrent;
using System.Diagnostics;

namespace GestaoRestaurante.Application.Common.Monitoring;

/// <summary>
/// Sistema de métricas personalizadas para monitoramento da aplicação
/// </summary>
public interface IApplicationMetrics
{
    void IncrementCounter(string name, Dictionary<string, string>? tags = null);
    void RecordGauge(string name, double value, Dictionary<string, string>? tags = null);
    void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null);
    void RecordTiming(string name, TimeSpan duration, Dictionary<string, string>? tags = null);
    Task<MetricsSummary> GetMetricsSummaryAsync();
    void Reset();
}

/// <summary>
/// Implementação em memória das métricas de aplicação
/// </summary>
public class ApplicationMetrics : IApplicationMetrics
{
    private readonly ConcurrentDictionary<string, Counter> _counters = new();
    private readonly ConcurrentDictionary<string, Gauge> _gauges = new();
    private readonly ConcurrentDictionary<string, Histogram> _histograms = new();
    private readonly DateTime _startTime = DateTime.UtcNow;

    public void IncrementCounter(string name, Dictionary<string, string>? tags = null)
    {
        var key = CreateKey(name, tags);
        _counters.AddOrUpdate(key, 
            new Counter(name, tags), 
            (k, existing) => { existing.Increment(); return existing; });
    }

    public void RecordGauge(string name, double value, Dictionary<string, string>? tags = null)
    {
        var key = CreateKey(name, tags);
        _gauges.AddOrUpdate(key,
            new Gauge(name, value, tags),
            (k, existing) => { existing.Set(value); return existing; });
    }

    public void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null)
    {
        var key = CreateKey(name, tags);
        _histograms.AddOrUpdate(key,
            new Histogram(name, tags),
            (k, existing) => existing);
        
        _histograms[key].Record(value);
    }

    public void RecordTiming(string name, TimeSpan duration, Dictionary<string, string>? tags = null)
    {
        RecordHistogram($"{name}.duration_ms", duration.TotalMilliseconds, tags);
    }

    public Task<MetricsSummary> GetMetricsSummaryAsync()
    {
        var summary = new MetricsSummary
        {
            StartTime = _startTime,
            Uptime = DateTime.UtcNow - _startTime,
            Counters = _counters.Values.Select(c => c.ToSnapshot()).ToList(),
            Gauges = _gauges.Values.Select(g => g.ToSnapshot()).ToList(),
            Histograms = _histograms.Values.Select(h => h.ToSnapshot()).ToList()
        };

        return Task.FromResult(summary);
    }

    public void Reset()
    {
        _counters.Clear();
        _gauges.Clear();
        _histograms.Clear();
    }

    private static string CreateKey(string name, Dictionary<string, string>? tags)
    {
        if (tags == null || !tags.Any())
            return name;

        var tagString = string.Join(",", tags.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return $"{name}|{tagString}";
    }
}

/// <summary>
/// Counter thread-safe
/// </summary>
public class Counter
{
    private long _value = 0;
    private readonly object _lock = new();

    public Counter(string name, Dictionary<string, string>? tags = null)
    {
        Name = name;
        Tags = tags ?? new Dictionary<string, string>();
        CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; }
    public Dictionary<string, string> Tags { get; }
    public DateTime CreatedAt { get; }
    public long Value => _value;

    public void Increment()
    {
        lock (_lock)
        {
            _value++;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public DateTime LastUpdated { get; private set; }

    public CounterSnapshot ToSnapshot()
    {
        return new CounterSnapshot
        {
            Name = Name,
            Tags = new Dictionary<string, string>(Tags),
            Value = Value,
            CreatedAt = CreatedAt,
            LastUpdated = LastUpdated
        };
    }
}

/// <summary>
/// Gauge para valores que podem subir ou descer
/// </summary>
public class Gauge
{
    private double _value;
    private readonly object _lock = new();

    public Gauge(string name, double initialValue = 0, Dictionary<string, string>? tags = null)
    {
        Name = name;
        _value = initialValue;
        Tags = tags ?? new Dictionary<string, string>();
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public string Name { get; }
    public Dictionary<string, string> Tags { get; }
    public DateTime CreatedAt { get; }
    public double Value => _value;
    public DateTime LastUpdated { get; private set; }

    public void Set(double value)
    {
        lock (_lock)
        {
            _value = value;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public GaugeSnapshot ToSnapshot()
    {
        return new GaugeSnapshot
        {
            Name = Name,
            Tags = new Dictionary<string, string>(Tags),
            Value = Value,
            CreatedAt = CreatedAt,
            LastUpdated = LastUpdated
        };
    }
}

/// <summary>
/// Histogram para distribuição de valores
/// </summary>
public class Histogram
{
    private readonly List<double> _values = new();
    private readonly object _lock = new();

    public Histogram(string name, Dictionary<string, string>? tags = null)
    {
        Name = name;
        Tags = tags ?? new Dictionary<string, string>();
        CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; }
    public Dictionary<string, string> Tags { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastUpdated { get; private set; }

    public void Record(double value)
    {
        lock (_lock)
        {
            _values.Add(value);
            LastUpdated = DateTime.UtcNow;

            // Manter apenas últimos 1000 valores para evitar memory leak
            if (_values.Count > 1000)
            {
                _values.RemoveRange(0, 100);
            }
        }
    }

    public HistogramSnapshot ToSnapshot()
    {
        lock (_lock)
        {
            if (!_values.Any())
            {
                return new HistogramSnapshot
                {
                    Name = Name,
                    Tags = new Dictionary<string, string>(Tags),
                    Count = 0,
                    CreatedAt = CreatedAt,
                    LastUpdated = LastUpdated
                };
            }

            var sortedValues = _values.OrderBy(v => v).ToArray();
            
            return new HistogramSnapshot
            {
                Name = Name,
                Tags = new Dictionary<string, string>(Tags),
                Count = sortedValues.Length,
                Min = sortedValues.Min(),
                Max = sortedValues.Max(),
                Mean = sortedValues.Average(),
                P50 = GetPercentile(sortedValues, 0.50),
                P95 = GetPercentile(sortedValues, 0.95),
                P99 = GetPercentile(sortedValues, 0.99),
                CreatedAt = CreatedAt,
                LastUpdated = LastUpdated
            };
        }
    }

    private static double GetPercentile(double[] sortedValues, double percentile)
    {
        if (!sortedValues.Any()) return 0;
        
        var index = (int)Math.Ceiling(percentile * sortedValues.Length) - 1;
        if (index < 0) index = 0;
        if (index >= sortedValues.Length) index = sortedValues.Length - 1;
        
        return sortedValues[index];
    }
}

/// <summary>
/// Snapshots para serialização
/// </summary>
public class MetricsSummary
{
    public DateTime StartTime { get; set; }
    public TimeSpan Uptime { get; set; }
    public List<CounterSnapshot> Counters { get; set; } = new();
    public List<GaugeSnapshot> Gauges { get; set; } = new();
    public List<HistogramSnapshot> Histograms { get; set; } = new();
}

public class CounterSnapshot
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public long Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class GaugeSnapshot
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public double Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class HistogramSnapshot
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public int Count { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Mean { get; set; }
    public double P50 { get; set; }
    public double P95 { get; set; }
    public double P99 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Extensões para facilitar uso das métricas
/// </summary>
public static class MetricsExtensions
{
    /// <summary>
    /// Mede tempo de execução automaticamente
    /// </summary>
    public static IDisposable Time(this IApplicationMetrics metrics, string name, Dictionary<string, string>? tags = null)
    {
        return new TimingScope(metrics, name, tags);
    }

    private class TimingScope : IDisposable
    {
        private readonly IApplicationMetrics _metrics;
        private readonly string _name;
        private readonly Dictionary<string, string>? _tags;
        private readonly Stopwatch _stopwatch;

        public TimingScope(IApplicationMetrics metrics, string name, Dictionary<string, string>? tags)
        {
            _metrics = metrics;
            _name = name;
            _tags = tags;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _metrics.RecordTiming(_name, _stopwatch.Elapsed, _tags);
        }
    }
}