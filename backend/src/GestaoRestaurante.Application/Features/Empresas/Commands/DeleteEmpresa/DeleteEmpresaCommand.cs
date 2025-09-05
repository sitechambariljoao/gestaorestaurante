using GestaoRestaurante.Application.Common.Commands;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.DeleteEmpresa;

/// <summary>
/// Command para excluir uma empresa
/// </summary>
public sealed record DeleteEmpresaCommand(Guid Id) : ICommand;