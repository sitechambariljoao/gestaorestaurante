using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Caching;
using System.Text.Json;
using System.Collections;

namespace GestaoRestaurante.Application.Common.Caching;

/// <summary>
/// Implementação do cache usando IMemoryCache
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly MemoryCacheEntryOptions _defaultOptions;

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _defaultOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(5),
            Priority = CacheItemPriority.Normal
        };
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = _memoryCache.Get<T>(key);
            
            if (value != null)
            {
                _logger.LogDebug("Cache HIT para chave: {Key}", key);
            }
            else
            {
                _logger.LogDebug("Cache MISS para chave: {Key}", key);
            }

            return Task.FromResult(value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao obter item do cache: {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = _defaultOptions;
            
            if (expiration.HasValue)
            {
                options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration.Value,
                    Priority = CacheItemPriority.Normal
                };
            }

            _memoryCache.Set(key, value, options);
            
            _logger.LogDebug("Item adicionado ao cache: {Key} - Expiração: {Expiration}", 
                key, expiration?.ToString() ?? "Padrão");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao definir item no cache: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Item removido do cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao remover item do cache: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        return RemovePatternInternalAsync(pattern, cancellationToken);
    }

    public Task RemovePatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        return RemovePatternInternalAsync(pattern, cancellationToken);
    }

    private Task RemovePatternInternalAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // MemoryCache não suporta remoção por padrão nativamente
            // Esta implementação é limitada - para padrões complexos, considere usar Redis
            
            if (_memoryCache is MemoryCache mc)
            {
                var field = typeof(MemoryCache).GetField("_coherentState", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field?.GetValue(mc) is IDictionary coherentState)
                {
                    var keysToRemove = new List<object>();
                    
                    foreach (DictionaryEntry entry in coherentState)
                    {
                        if (entry.Key.ToString()?.Contains(pattern) == true)
                        {
                            keysToRemove.Add(entry.Key);
                        }
                    }

                    foreach (var key in keysToRemove)
                    {
                        _memoryCache.Remove(key);
                    }

                    _logger.LogDebug("Removidos {Count} itens do cache com padrão: {Pattern}", 
                        keysToRemove.Count, pattern);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao remover itens por padrão: {Pattern}", pattern);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = _memoryCache.TryGetValue(key, out _);
            return Task.FromResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao verificar existência no cache: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var cachedItem = await GetAsync<T>(key, cancellationToken);
        
        if (cachedItem != null)
        {
            return cachedItem;
        }

        var item = await getItem();
        
        if (item != null)
        {
            await SetAsync(key, item, expiration, cancellationToken);
        }

        return item;
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> keyValuePairs, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        foreach (var kvp in keyValuePairs)
        {
            await SetAsync(kvp.Key, kvp.Value, expiration, cancellationToken);
        }
    }

    public async Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        var result = new Dictionary<string, T?>();
        
        foreach (var key in keys)
        {
            var value = await GetAsync<T>(key, cancellationToken);
            result[key] = value;
        }

        return result;
    }
}