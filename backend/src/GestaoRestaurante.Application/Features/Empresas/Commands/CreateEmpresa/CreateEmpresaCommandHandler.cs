using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Services;
using GestaoRestaurante.Domain.ValueObjects;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.CreateEmpresa;

/// <summary>
/// Handler para o comando de criação de empresa
/// </summary>
public sealed class CreateEmpresaCommandHandler : ICommandHandler<CreateEmpresaCommand, EmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IEmpresaDomainService _empresaDomainService;
    private readonly CreateEmpresaDbValidator _dbValidator;
    private readonly IMapper _mapper;

    public CreateEmpresaCommandHandler(
        IEmpresaRepository empresaRepository,
        IEmpresaDomainService empresaDomainService,
        CreateEmpresaDbValidator dbValidator,
        IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _empresaDomainService = empresaDomainService;
        _dbValidator = dbValidator;
        _mapper = mapper;
    }

    public async Task<Result<EmpresaDto>> Handle(CreateEmpresaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar regras de domínio
            var domainValidationResult = await _empresaDomainService.ValidateEmpresaCreationAsync(
                request.Cnpj, request.Email);
            
            if (!domainValidationResult.IsSuccess)
                return Result<EmpresaDto>.Failure(domainValidationResult.Errors);

            // Validar regras de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(
                new CreateEmpresaDto
                {
                    RazaoSocial = request.RazaoSocial,
                    NomeFantasia = request.NomeFantasia,
                    Cnpj = request.Cnpj,
                    Email = request.Email,
                    Telefone = request.Telefone,
                    Endereco = request.Endereco
                });

            if (!dbValidationResult.IsValid)
                return Result<EmpresaDto>.Failure(dbValidationResult.Errors.Select(e => e.ErrorMessage));

            // Criar value objects
#pragma warning disable CS8604 // Possible null reference argument.

            var endereco = new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Estado,
                request.Endereco.Cep,
                request.Endereco.Complemento
            );
#pragma warning restore CS8604 // Possible null reference argument.

            // Criar empresa

            var empresa = new Empresa(
                request.RazaoSocial,
                request.NomeFantasia,
                request.Cnpj,
                request.Email,
                endereco,
                request.Telefone
            );

            // Validar invariantes do aggregate
            empresa.ValidateInvariants();

            // Persistir
            await _empresaRepository.AddAsync(empresa);
            await _empresaRepository.SaveChangesAsync();

            // Mapear para DTO
            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result<EmpresaDto>.Success(empresaDto);
        }
        catch (Exception ex)
        {
            return Result<EmpresaDto>.Failure($"Erro ao criar empresa: {ex.Message}");
        }
    }
}