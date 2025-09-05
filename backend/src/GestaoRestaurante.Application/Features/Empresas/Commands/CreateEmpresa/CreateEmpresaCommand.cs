using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.CreateEmpresa;

/// <summary>
/// Command para criar uma nova empresa
/// </summary>
public sealed record CreateEmpresaCommand(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Email,
    string? Telefone,
    EnderecoDto Endereco
) : ICommand<EmpresaDto>;