using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.CreateFilial;

public class CreateFilialCommandHandler : ICommandHandler<CreateFilialCommand, FilialDto>
{
    private readonly IFilialRepository _filialRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateFilialDto> _validator;

    public CreateFilialCommandHandler(
        IFilialRepository filialRepository,
        IEmpresaRepository empresaRepository,
        IMapper mapper,
        IValidator<CreateFilialDto> validator)
    {
        _filialRepository = filialRepository;
        _empresaRepository = empresaRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<FilialDto>> Handle(CreateFilialCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var createDto = new CreateFilialDto
        {
            EmpresaId = request.EmpresaId,
            Nome = request.Nome,
            Cnpj = request.Cnpj,
            Email = request.Email,
            Telefone = request.Telefone,
            Endereco = request.Endereco
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<FilialDto>.Failure(errors);
        }

        try
        {
            // Verify Empresa exists
            var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaId);
            if (empresa == null)
            {
                return Result<FilialDto>.Failure(BusinessRuleMessages.EMPRESA_NAO_ENCONTRADA);
            }

            // Check uniqueness if CNPJ provided
            if (!string.IsNullOrWhiteSpace(request.Cnpj))
            {
                var existingByCnpj = await _filialRepository.GetByCnpjAsync(request.Cnpj);
                if (existingByCnpj != null)
                {
                    return Result<FilialDto>.Failure($"Já existe uma filial com CNPJ '{request.Cnpj}'");
                }
            }

            // Check uniqueness if Email provided
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingByEmail = await _filialRepository.GetByEmailAsync(request.Email);
                if (existingByEmail != null)
                {
                    return Result<FilialDto>.Failure($"Já existe uma filial com email '{request.Email}'");
                }
            }

            // Create entity usando o construtor ou propriedades corretas
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

            var filial = new Filial(
                request.EmpresaId,
                request.Nome,
                false, // matriz
                request.Cnpj, // cnpjFilial
                request.Email,
                request.Telefone,
                endereco // enderecoFilial
            );

            await _filialRepository.AddAsync(filial);

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