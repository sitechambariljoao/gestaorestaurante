using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.UpdateEmpresa;

/// <summary>
/// Command para atualizar uma empresa existente
/// </summary>
public sealed record UpdateEmpresaCommand(
    Guid Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Email,
    string? Telefone,
    EnderecoDto Endereco
) : ICommand<EmpresaDto>;