using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.CreateFilial;

public record CreateFilialCommand(
    Guid EmpresaId,
    string Nome,
    string? Cnpj,
    string? Email,
    string? Telefone,
    EnderecoDto? Endereco
) : ICommand<FilialDto>;