using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Services;
using GestaoRestaurante.Domain.ValueObjects;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.UpdateEmpresa;

/// <summary>
/// Handler para o comando de atualização de empresa
/// </summary>
public sealed class UpdateEmpresaCommandHandler : ICommandHandler<UpdateEmpresaCommand, EmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IEmpresaDomainService _empresaDomainService;
    private readonly UpdateEmpresaDbValidator _dbValidator;
    private readonly IMapper _mapper;

    public UpdateEmpresaCommandHandler(
        IEmpresaRepository empresaRepository,
        IEmpresaDomainService empresaDomainService,
        UpdateEmpresaDbValidator dbValidator,
        IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _empresaDomainService = empresaDomainService;
        _dbValidator = dbValidator;
        _mapper = mapper;
    }

    public async Task<Result<EmpresaDto>> Handle(UpdateEmpresaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar empresa existente
            var empresa = await _empresaRepository.GetByIdAsync(request.Id);
            if (empresa == null)
                return Result<EmpresaDto>.Failure("Empresa não encontrada");

            // Validar regras de domínio (se mudou CNPJ ou email)
            if (empresa.Cnpj != request.Cnpj || empresa.Email != request.Email)
            {
                var domainValidationResult = await _empresaDomainService.ValidateEmpresaUpdateAsync(
                    request.Id, request.Cnpj, request.Email);
                
                if (!domainValidationResult.IsSuccess)
                    return Result<EmpresaDto>.Failure(domainValidationResult.Errors);
            }

            // Validar regras de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(
                new UpdateEmpresaDto
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
            var endereco = new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Estado,
                request.Endereco.Cep,
                request.Endereco.Complemento
            );

            // Atualizar empresa
            empresa.AtualizarDados(
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
            _empresaRepository.Update(empresa);
            await _empresaRepository.SaveChangesAsync();

            // Mapear para DTO
            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result<EmpresaDto>.Success(empresaDto);
        }
        catch (Exception ex)
        {
            return Result<EmpresaDto>.Failure($"Erro ao atualizar empresa: {ex.Message}");
        }
    }
}