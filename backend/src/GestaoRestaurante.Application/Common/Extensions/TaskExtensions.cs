using System.Runtime.CompilerServices;

namespace GestaoRestaurante.Application.Common.Extensions;

/// <summary>
/// Extensions para otimização de Tasks
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Configura await para não capturar o contexto de sincronização
    /// </summary>
    public static ConfiguredTaskAwaitable ConfigureAwaitOptimized(this Task task)
    {
        return task.ConfigureAwait(false);
    }

    /// <summary>
    /// Configura await para não capturar o contexto de sincronização para Task<T>
    /// </summary>
    public static ConfiguredTaskAwaitable<T> ConfigureAwaitOptimized<T>(this Task<T> task)
    {
        return task.ConfigureAwait(false);
    }

    /// <summary>
    /// Executa múltiplas tasks concorrentemente
    /// </summary>
    public static async Task WhenAllOptimized(params Task[] tasks)
    {
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    /// Executa múltiplas tasks concorrentemente com resultado
    /// </summary>
    public static async Task<T[]> WhenAllOptimized<T>(params Task<T>[] tasks)
    {
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    /// Timeout para task
    /// </summary>
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(timeout);
        
        return await task.WaitAsync(cancellationTokenSource.Token).ConfigureAwait(false);
    }
}