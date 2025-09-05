using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Specifications;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

/// <summary>
/// Avalia specifications e aplica aos IQueryable do Entity Framework
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Aplica uma specification a um IQueryable
    /// </summary>
    public static IQueryable<T> ApplySpecification<T>(IQueryable<T> inputQuery, ISpecification<T> specification) where T : class
    {
        var query = inputQuery;

        // Aplicar a expressão da specification
        if (specification != null)
        {
            query = query.Where(specification.ToExpression());
        }

        return query;
    }

    /// <summary>
    /// Aplica múltiplas specifications a um IQueryable usando operador AND
    /// </summary>
    public static IQueryable<T> ApplySpecifications<T>(IQueryable<T> inputQuery, params ISpecification<T>[] specifications) where T : class
    {
        var query = inputQuery;

        foreach (var spec in specifications.Where(s => s != null))
        {
            query = query.Where(spec.ToExpression());
        }

        return query;
    }

    /// <summary>
    /// Aplica paginação a um IQueryable
    /// </summary>
    public static IQueryable<T> ApplyPaging<T>(IQueryable<T> inputQuery, int pageIndex, int pageSize) where T : class
    {
        if (pageIndex < 0) pageIndex = 0;
        if (pageSize <= 0) pageSize = 10;

        return inputQuery
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Aplica ordenação a um IQueryable
    /// </summary>
    public static IQueryable<T> ApplyOrdering<T>(IQueryable<T> inputQuery, string orderBy, bool descending = false) where T : class
    {
        if (string.IsNullOrEmpty(orderBy))
            return inputQuery;

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
        
        try
        {
            var property = System.Linq.Expressions.Expression.Property(parameter, orderBy);
            var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = System.Linq.Expressions.Expression.Call(
                typeof(Queryable), methodName,
                new Type[] { typeof(T), property.Type },
                inputQuery.Expression, System.Linq.Expressions.Expression.Quote(lambda));

            return inputQuery.Provider.CreateQuery<T>(resultExpression);
        }
        catch
        {
            // Se a propriedade não existir, retorna a query original
            return inputQuery;
        }
    }
}