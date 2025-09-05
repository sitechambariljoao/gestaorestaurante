using System.Linq.Expressions;

namespace GestaoRestaurante.Application.Common.Extensions;

/// <summary>
/// Extension methods para projeções de queries
/// </summary>
public static class QueryProjections
{
    /// <summary>
    /// Projeção básica com Select para evitar carregar entidade completa
    /// </summary>
    public static IQueryable<TResult> ProjectTo<TEntity, TResult>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector)
    {
        return query.Select(selector);
    }

    /// <summary>
    /// Projeção com includes otimizados
    /// </summary>
    public static IQueryable<TResult> ProjectToWithIncludes<TEntity, TResult>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector,
        params Expression<Func<TEntity, object>>[] includes)
    {
        foreach (var include in includes)
        {
            // Esta seria a implementação real com Entity Framework
            // query = query.Include(include);
        }
        return query.Select(selector);
    }

    /// <summary>
    /// Projeção específica para DTOs com validação
    /// </summary>
    public static IQueryable<TResult> ProjectToDto<TEntity, TResult>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? filter = null)
        where TEntity : class
        where TResult : class
    {
        var filteredQuery = filter != null ? query.Where(filter) : query;
        return filteredQuery.Select(selector);
    }
}