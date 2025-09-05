using MediatR;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Common.Queries;

/// <summary>
/// Interface base para queries
/// </summary>
/// <typeparam name="TResponse">Tipo do valor de retorno</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}