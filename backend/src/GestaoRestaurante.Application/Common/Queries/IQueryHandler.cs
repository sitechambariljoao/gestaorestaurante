using MediatR;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Common.Queries;

/// <summary>
/// Interface base para handlers de queries
/// </summary>
/// <typeparam name="TQuery">Tipo da query</typeparam>
/// <typeparam name="TResponse">Tipo do valor de retorno</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}