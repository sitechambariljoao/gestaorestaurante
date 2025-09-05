using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Exceptions;
using GestaoRestaurante.Domain.Constants;

namespace GestaoRestaurante.Application.Services;

public class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IFilialRepository _filialRepository;
    private readonly IMapper _mapper;
    private readonly CreateEmpresaDbValidator _createDbValidator;
    private readonly UpdateEmpresaDbValidator _updateDbValidator;
    private readonly ILogger<EmpresaService> _logger;

    public EmpresaService(
        IEmpresaRepository empresaRepository,
        IFilialRepository filialRepository,
        IMapper mapper,
        CreateEmpresaDbValidator createDbValidator,
        UpdateEmpresaDbValidator updateDbValidator,
        ILogger<EmpresaService> logger)
    {
        _empresaRepository = empresaRepository;
        _filialRepository = filialRepository;
        _mapper = mapper;
        _createDbValidator = createDbValidator;
        _updateDbValidator = updateDbValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<EmpresaDto>>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Buscando todas as empresas ativas");

            var empresas = await _empresaRepository.GetAllWithFiliaisAsync();
            var empresasDto = _mapper.Map<List<EmpresaDto>>(empresas);

            _logger.LogInformation("Encontradas {Count} empresas", empresasDto.Count);
            return ServiceResult<IEnumerable<EmpresaDto>>.SuccessResult(empresasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar empresas");
            return ServiceResult<IEnumerable<EmpresaDto>>.ErrorResult(BusinessRuleMessages.System.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmpresaDto>> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando empresa com ID: {EmpresaId}", id);

            var empresa = await _empresaRepository.GetByIdWithFiliaisAsync(id);

            if (empresa == null)
            {
                _logger.LogWarning("Empresa não encontrada: {EmpresaId}", id);
                return ServiceResult<EmpresaDto>.ErrorResult(
                    string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Empresa"));
            }

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return ServiceResult<EmpresaDto>.SuccessResult(empresaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar empresa {EmpresaId}", id);
            return ServiceResult<EmpresaDto>.ErrorResult(BusinessRuleMessages.System.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmpresaDto>> CreateAsync(CreateEmpresaDto createDto)
    {
        try
        {
            _logger.LogInformation("Criando nova empresa: {NomeFantasia}", createDto.NomeFantasia);

            // Validar dados usando FluentValidation
            var validationResult = await ValidateCreateAsync(createDto);
            if (validationResult.IsFailure)
            {
                return ServiceResult<EmpresaDto>.ValidationErrorResult(validationResult.Errors.ToList());
            }

            // Usar construtor da entidade para criar empresa com validação de domínio
            var endereco = _mapper.Map<Domain.ValueObjects.Endereco>(createDto.Endereco);
            var empresa = new Empresa(
                createDto.RazaoSocial,
                createDto.NomeFantasia,
                createDto.Cnpj,
                createDto.Email,
                endereco,
                createDto.Telefone
            );

            empresa.Id = Guid.NewGuid();

            await _empresaRepository.AddAsync(empresa);
            await _empresaRepository.SaveChangesAsync();

            _logger.LogInformation("Empresa criada com sucesso: {EmpresaId}", empresa.Id);

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return ServiceResult<EmpresaDto>.SuccessResult(empresaDto);
        }
        catch (Domain.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Erro de validação ao criar empresa: {Errors}", string.Join("; ", ex.Errors.SelectMany(kvp => kvp.Value)));
            return ServiceResult<EmpresaDto>.ValidationErrorResult(ex.Errors.SelectMany(kvp => kvp.Value).ToList());
        }
        catch (DuplicateEntityException ex)
        {
            _logger.LogWarning("Tentativa de criar empresa duplicada: {Message}", ex.Message);
            return ServiceResult<EmpresaDto>.ErrorResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar empresa");
            return ServiceResult<EmpresaDto>.ErrorResult(BusinessRuleMessages.System.DatabaseError);
        }
    }

    private async Task<Result> ValidateCreateAsync(CreateEmpresaDto createDto)
    {
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public async Task<ServiceResult<EmpresaDto>> UpdateAsync(Guid id, UpdateEmpresaDto updateDto)
    {
        _logger.LogInformation("Atualizando empresa: {EmpresaId}", id);

        var empresa = await _empresaRepository.GetByIdAsync(id);
        if (empresa == null)
        {
            _logger.LogWarning("Empresa não encontrada para atualização: {EmpresaId}", id);
            return ServiceResult<EmpresaDto>.ErrorResult("Empresa não encontrada");
        }

        // Validar dados usando FluentValidation
        _updateDbValidator.SetExcludeId(id);
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<EmpresaDto>.ValidationErrorResult(errors);
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, empresa);

        _empresaRepository.Update(empresa);
        await _empresaRepository.SaveChangesAsync();

        _logger.LogInformation("Empresa atualizada com sucesso: {EmpresaId}", id);

        var empresaDto = _mapper.Map<EmpresaDto>(empresa);
        return ServiceResult<EmpresaDto>.SuccessResult(empresaDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando empresa: {EmpresaId}", id);

        var empresa = await _empresaRepository.GetByIdWithFiliaisAsync(id);

        if (empresa == null)
        {
            _logger.LogWarning("Empresa não encontrada para desativação: {EmpresaId}", id);
            return ServiceResult<bool>.ErrorResult("Empresa não encontrada");
        }

        // Verificar se empresa tem filiais ativas
        var filiaisAtivas = empresa.Filiais.Count(f => f.Ativa);
        if (filiaisAtivas > 0)
        {
            _logger.LogWarning("Tentativa de desativar empresa com filiais ativas: {EmpresaId}, Filiais: {Count}", id, filiaisAtivas);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar empresa com {filiaisAtivas} filial(is) ativa(s)");
        }

        _empresaRepository.SoftDelete(empresa);
        await _empresaRepository.SaveChangesAsync();

        _logger.LogInformation("Empresa desativada com sucesso: {EmpresaId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<IEnumerable<FilialDto>>> GetFiliaisAsync(Guid empresaId)
    {
        _logger.LogInformation("Buscando filiais da empresa: {EmpresaId}", empresaId);

        var empresa = await _empresaRepository.GetByIdAsync(empresaId);

        if (empresa == null)
        {
            return ServiceResult<IEnumerable<FilialDto>>.ErrorResult("Empresa não encontrada");
        }

        var filiais = await _filialRepository.GetByEmpresaIdAsync(empresaId);

        var filiaisDto = _mapper.Map<List<FilialDto>>(filiais);

        _logger.LogInformation("Encontradas {Count} filiais para empresa {EmpresaId}", filiaisDto.Count, empresaId);
        return ServiceResult<IEnumerable<FilialDto>>.SuccessResult(filiaisDto);
    }

    public async Task<ServiceResult<bool>> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _empresaRepository.ExistsByCnpjAsync(cnpj, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByEmailAsync(string email, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _empresaRepository.ExistsByEmailAsync(email, excludeId));
    }


}