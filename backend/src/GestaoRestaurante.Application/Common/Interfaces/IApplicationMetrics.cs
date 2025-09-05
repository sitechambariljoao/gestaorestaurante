namespace GestaoRestaurante.Application.Common.Interfaces;

/// <summary>
/// Interface para sistema de métricas da aplicação
/// </summary>
public interface IApplicationMetrics
{
    /// <summary>
    /// Incrementa um contador
    /// </summary>
    void IncrementCounter(string name, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Registra um valor de gauge
    /// </summary>
    void SetGauge(string name, double value, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Registra um tempo de execução
    /// </summary>
    void RecordTimer(string name, TimeSpan duration, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Registra um histogram
    /// </summary>
    void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Registra um valor numérico
    /// </summary>
    void RecordValue(string name, double value, Dictionary<string, string>? tags = null);
}