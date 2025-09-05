namespace GestaoRestaurante.Application.Common.Interfaces.Segregated;

/// <summary>
/// Interface segregada para operações de leitura apenas
/// </summary>
public interface IReadOnlyService<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> ExistsAsync(TKey id);
    Task<int> CountAsync();
}

/// <summary>
/// Interface segregada para operações de escrita apenas
/// </summary>
public interface IWriteOnlyService<TEntity, TKey> where TEntity : class
{
    Task<TKey> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}

/// <summary>
/// Interface segregada para operações de busca complexa
/// </summary>
public interface ISearchableService<TEntity, TSearchParams> where TEntity : class
{
    Task<IEnumerable<TEntity>> SearchAsync(TSearchParams searchParams);
    Task<(IEnumerable<TEntity> Items, int TotalCount)> SearchPagedAsync(TSearchParams searchParams, int page, int pageSize);
}

/// <summary>
/// Interface segregada para operações de cache
/// </summary>
public interface ICacheableService<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetFromCacheAsync(TKey id);
    Task SetCacheAsync(TKey id, TEntity entity, TimeSpan? expiration = null);
    Task InvalidateCacheAsync(TKey id);
    Task InvalidateAllCacheAsync();
}

/// <summary>
/// Interface segregada para validação de negócio
/// </summary>
public interface IValidatableService<TEntity>
{
    Task<bool> ValidateAsync(TEntity entity);
    Task<IEnumerable<string>> ValidateAndGetErrorsAsync(TEntity entity);
    Task<bool> CanDeleteAsync(object id);
}

/// <summary>
/// Interface segregada para auditoria
/// </summary>
public interface IAuditableService<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<AuditEntry>> GetAuditHistoryAsync(TKey id);
    Task LogOperationAsync(TKey id, string operation, string details);
}

/// <summary>
/// Interface segregada para operações de importação/exportação
/// </summary>
public interface IImportExportService<TEntity>
{
    Task<ImportResult> ImportAsync(Stream data, string format);
    Task<Stream> ExportAsync(IEnumerable<TEntity> entities, string format);
    Task<bool> ValidateImportAsync(Stream data, string format);
}

/// <summary>
/// Resultado de operação de importação
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int ErrorCount { get; set; }
    public IList<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Entrada de auditoria
/// </summary>
public class AuditEntry
{
    public DateTime Timestamp { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}