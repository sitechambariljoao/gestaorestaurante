using MediatR;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Common.Commands;

/// <summary>
/// Interface base para commands que não retornam valor
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Interface base para commands que retornam valor
/// </summary>
/// <typeparam name="TResponse">Tipo do valor de retorno</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}