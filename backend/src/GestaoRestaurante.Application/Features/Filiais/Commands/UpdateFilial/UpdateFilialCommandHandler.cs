using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.UpdateFilial;

public class UpdateFilialCommandHandler : ICommandHandler<UpdateFilialCommand, FilialDto>
{
    private readonly IFilialRepository _filialRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateFilialDto> _validator;

    public UpdateFilialCommandHandler(
        IFilialRepository filialRepository,
        IMapper mapper,
        IValidator<UpdateFilialDto> validator)
    {
        _filialRepository = filialRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<FilialDto>> Handle(UpdateFilialCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var updateDto = new UpdateFilialDto
        {
            Nome = request.Nome,
            Cnpj = request.Cnpj,
            Email = request.Email,
            Telefone = request.Telefone,
            Endereco = request.Endereco
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<FilialDto>.Failure(errors);
        }

        try
        {
            // Find existing filial
            var filial = await _filialRepository.GetByIdAsync(request.Id);
            if (filial == null)
            {
                return Result<FilialDto>.Failure(BusinessRuleMessages.FILIAL_NAO_ENCONTRADA);
            }

            // Check uniqueness if CNPJ provided (excluding current filial)
            if (!string.IsNullOrWhiteSpace(request.Cnpj))
            {
                var existingByCnpj = await _filialRepository.GetByCnpjAsync(request.Cnpj);
                if (existingByCnpj != null && existingByCnpj.Id != request.Id)
                {
                    return Result<FilialDto>.Failure($"Já existe uma filial com CNPJ '{request.Cnpj}'");
                }
            }

            // Check uniqueness if Email provided (excluding current filial)
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingByEmail = await _filialRepository.GetByEmailAsync(request.Email);
                if (existingByEmail != null && existingByEmail.Id != request.Id)
                {
                    return Result<FilialDto>.Failure($"Já existe uma filial com email '{request.Email}'");
                }
            }

            // Update entity usando método de domínio
            var endereco = request.Endereco != null 
                ? new Endereco(
                    request.Endereco.Logradouro,
                    request.Endereco.Numero,
                    request.Endereco.Complemento,
                    request.Endereco.Bairro,
                    request.Endereco.Cidade,
                    request.Endereco.Estado,
                    request.Endereco.Cep)
                : null;

            filial.AtualizarDados(
                filial.EmpresaId,
                request.Nome,
                filial.Matriz,
                request.Cnpj,
                request.Email,
                request.Telefone,
                endereco
            );

            await _filialRepository.UpdateAsync(filial);

            var result = await _filialRepository.GetByIdAsync(filial.Id);
            var filialDto = _mapper.Map<FilialDto>(result);

            return Result<FilialDto>.Success(filialDto);
        }
        catch (Exception ex)
        {
            return Result<FilialDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}