using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;

namespace GestaoRestaurante.Application.Common.Performance;

/// <summary>
/// Otimizações de performance e padrões de alocação eficiente
/// </summary>
public static class PerformanceOptimizations
{
    /// <summary>
    /// Pool de StringBuilder para evitar alocações desnecessárias
    /// </summary>
    private static readonly ObjectPool<System.Text.StringBuilder> StringBuilderPool = 
        new DefaultObjectPoolProvider().CreateStringBuilderPool();

    /// <summary>
    /// Executa operação com StringBuilder pooled
    /// </summary>
    public static string WithPooledStringBuilder(Func<System.Text.StringBuilder, string> operation)
    {
        var sb = StringBuilderPool.Get();
        try
        {
            return operation(sb);
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// Executa múltiplas tarefas assíncronas com controle de concorrência
    /// </summary>
    public static async Task<T[]> ExecuteConcurrentAsync<T>(
        IEnumerable<Func<Task<T>>> tasks,
        int maxConcurrency = 10,
        CancellationToken cancellationToken = default)
    {
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        var results = new List<Task<T>>();

        foreach (var task in tasks)
        {
            results.Add(ExecuteWithSemaphoreAsync(task, semaphore, cancellationToken));
        }

        return await Task.WhenAll(results);
    }

    private static async Task<T> ExecuteWithSemaphoreAsync<T>(
        Func<Task<T>> task,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await task();
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Cache local de alta performance para operações frequentes
    /// </summary>
    public class HighPerformanceCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheEntry<TValue>> _cache = new();
        private readonly ReaderWriterLockSlim _lock = new();
        private readonly TimeSpan _defaultExpiration;

        public HighPerformanceCache(TimeSpan defaultExpiration)
        {
            _defaultExpiration = defaultExpiration;
        }

        public bool TryGet(TKey key, out TValue? value)
        {
            _lock.EnterReadLock();
            try
            {
                if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
                {
                    value = entry.Value;
                    return true;
                }
                
                value = default;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Set(TKey key, TValue value, TimeSpan? expiration = null)
        {
            var exp = expiration ?? _defaultExpiration;
            var entry = new CacheEntry<TValue>(value, DateTime.UtcNow.Add(exp));

            _lock.EnterWriteLock();
            try
            {
                _cache[key] = entry;
                
                // Limpeza de entradas expiradas (máximo 10% do cache)
                if (_cache.Count % 100 == 0)
                {
                    CleanupExpired();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void CleanupExpired()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.ExpiresAt < now)
                .Take(_cache.Count / 10) // Máximo 10%
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }

    private class CacheEntry<T>
    {
        public CacheEntry(T value, DateTime expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
        }

        public T Value { get; }
        public DateTime ExpiresAt { get; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Configuração otimizada para async/await
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable<T> ConfigureAwaitOptimized<T>(this Task<T> task)
    {
        return task.ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable ConfigureAwaitOptimized(this Task task)
    {
        return task.ConfigureAwait(false);
    }

    /// <summary>
    /// Batch processor para operações em lote eficientes
    /// </summary>
    public static async Task<IEnumerable<TResult>> ProcessInBatchesAsync<TItem, TResult>(
        IEnumerable<TItem> items,
        Func<IEnumerable<TItem>, Task<IEnumerable<TResult>>> batchProcessor,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var results = new List<TResult>();
        var batch = new List<TItem>(batchSize);

        foreach (var item in items)
        {
            batch.Add(item);
            
            if (batch.Count >= batchSize)
            {
                var batchResults = await batchProcessor(batch).ConfigureAwaitOptimized();
                results.AddRange(batchResults);
                batch.Clear();
                
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        // Processar último batch se houver items restantes
        if (batch.Count > 0)
        {
            var finalResults = await batchProcessor(batch).ConfigureAwaitOptimized();
            results.AddRange(finalResults);
        }

        return results;
    }
}