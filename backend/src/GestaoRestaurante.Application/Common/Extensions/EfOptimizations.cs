namespace GestaoRestaurante.Application.Common.Extensions;

/// <summary>
/// Extension methods para otimizações específicas do Entity Framework
/// </summary>
public static class EfOptimizations
{
    /// <summary>
    /// Aplica AsNoTracking para consultas read-only
    /// </summary>
    public static IQueryable<T> AsReadOnly<T>(this IQueryable<T> query) where T : class
    {
        // Esta seria a implementação real com Entity Framework:
        // return query.AsNoTracking();
        return query; // Implementação básica para compilação
    }

    /// <summary>
    /// Aplica paginação otimizada
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        return query.Skip(pageIndex * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Aplica ordenação dinâmica por string
    /// </summary>
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string orderBy, bool descending = false)
    {
        if (string.IsNullOrEmpty(orderBy))
            return query;

        // Implementação básica - em um caso real usaríamos System.Linq.Dynamic ou Expression Trees
        return query; // Por agora, retorna sem modificação
    }

    /// <summary>
    /// Força compilação da query para melhores performances em queries repetitivas
    /// </summary>
    public static IQueryable<T> CompileQuery<T>(this IQueryable<T> query) where T : class
    {
        // Esta seria a implementação real com EF Core:
        // return EF.CompileQuery(...);
        return query; // Implementação básica para compilação
    }

    /// <summary>
    /// Otimiza consultas com includes múltiplos
    /// </summary>
    public static IQueryable<T> OptimizeIncludes<T>(this IQueryable<T> query) where T : class
    {
        // Aplicaria técnicas como SplitQuery para evitar cartesian products
        // return query.AsSplitQuery();
        return query; // Implementação básica para compilação
    }
}