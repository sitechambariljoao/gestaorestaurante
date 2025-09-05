using MediatR;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Common.Commands;

/// <summary>
/// Interface base para handlers de commands que não retornam valor
/// </summary>
/// <typeparam name="TCommand">Tipo do command</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Interface base para handlers de commands que retornam valor
/// </summary>
/// <typeparam name="TCommand">Tipo do command</typeparam>
/// <typeparam name="TResponse">Tipo do valor de retorno</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}