using System.Linq.Expressions;

namespace GestaoRestaurante.Domain.Specifications;

/// <summary>
/// Interface base para especificações que podem ser utilizadas para filtros complexos
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Expressão que define a condição da especificação
    /// </summary>
    Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Verifica se uma entidade satisfaz a especificação
    /// </summary>
    bool IsSatisfiedBy(T entity);
}