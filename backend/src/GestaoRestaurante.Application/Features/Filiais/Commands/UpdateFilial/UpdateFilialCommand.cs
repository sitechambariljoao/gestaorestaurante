using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.UpdateFilial;

public record UpdateFilialCommand(
    Guid Id,
    string Nome,
    string? Cnpj,
    string? Email,
    string? Telefone,
    EnderecoDto? Endereco
) : ICommand<FilialDto>;