using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Services;

public class CentroCustoService : ICentroCustoService
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly CreateCentroCustoDbValidator _createDbValidator;
    private readonly UpdateCentroCustoDbValidator _updateDbValidator;
    private readonly ILogger<CentroCustoService> _logger;

    public CentroCustoService(
        ICentroCustoRepository centroCustoRepository,
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IMapper mapper,
        CreateCentroCustoDbValidator createDbValidator,
        UpdateCentroCustoDbValidator updateDbValidator,
        ILogger<CentroCustoService> logger)
    {
        _centroCustoRepository = centroCustoRepository;
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _mapper = mapper;
        _createDbValidator = createDbValidator;
        _updateDbValidator = updateDbValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<CentroCustoDto>>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os centros de custo ativos");

        var centrosCusto = await _centroCustoRepository.GetAllAsync();
        var centrosCustoDto = _mapper.Map<List<CentroCustoDto>>(centrosCusto);

        _logger.LogInformation("Encontrados {Count} centros de custo", centrosCustoDto.Count);
        return ServiceResult<IEnumerable<CentroCustoDto>>.SuccessResult(centrosCustoDto);
    }

    public async Task<ServiceResult<CentroCustoDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando centro de custo com ID: {CentroCustoId}", id);

        var centroCusto = await _centroCustoRepository.GetByIdAsync(id);

        if (centroCusto == null)
        {
            _logger.LogWarning("Centro de custo não encontrado: {CentroCustoId}", id);
            return ServiceResult<CentroCustoDto>.ErrorResult("Centro de custo não encontrado");
        }

        var centroCustoDto = _mapper.Map<CentroCustoDto>(centroCusto);
        return ServiceResult<CentroCustoDto>.SuccessResult(centroCustoDto);
    }

    public async Task<ServiceResult<IEnumerable<CentroCustoDto>>> GetBySubAgrupamentoIdAsync(Guid subAgrupamentoId)
    {
        _logger.LogInformation("Buscando centros de custo do sub-agrupamento: {SubAgrupamentoId}", subAgrupamentoId);

        var centrosCusto = await _centroCustoRepository.GetBySubAgrupamentoIdAsync(subAgrupamentoId);
        var centrosCustoDto = _mapper.Map<List<CentroCustoDto>>(centrosCusto);

        _logger.LogInformation("Encontrados {Count} centros de custo para sub-agrupamento {SubAgrupamentoId}", centrosCustoDto.Count, subAgrupamentoId);
        return ServiceResult<IEnumerable<CentroCustoDto>>.SuccessResult(centrosCustoDto);
    }

    public async Task<ServiceResult<IEnumerable<CentroCustoDto>>> GetByFilialIdAsync(Guid filialId)
    {
        _logger.LogInformation("Buscando centros de custo da filial: {FilialId}", filialId);

        var centrosCusto = await _centroCustoRepository.GetByFilialIdAsync(filialId);
        var centrosCustoDto = _mapper.Map<List<CentroCustoDto>>(centrosCusto);

        _logger.LogInformation("Encontrados {Count} centros de custo para filial {FilialId}", centrosCustoDto.Count, filialId);
        return ServiceResult<IEnumerable<CentroCustoDto>>.SuccessResult(centrosCustoDto);
    }

    public async Task<ServiceResult<CentroCustoDto>> CreateAsync(CreateCentroCustoDto createDto)
    {
        _logger.LogInformation("Criando novo centro de custo: {Nome} para sub-agrupamento {SubAgrupamentoId}", createDto.Nome, createDto.SubAgrupamentoId);

        // Validar dados usando FluentValidation
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<CentroCustoDto>.ValidationErrorResult(errors);
        }

        // Verificar se sub-agrupamento existe e está ativo
        var subAgrupamentoExists = await _subAgrupamentoRepository.ExistsAsync(createDto.SubAgrupamentoId);
        if (!subAgrupamentoExists)
        {
            return ServiceResult<CentroCustoDto>.ValidationErrorResult([$"Sub-agrupamento com ID {createDto.SubAgrupamentoId} não encontrado ou inativo"]);
        }

        var centroCusto = _mapper.Map<CentroCusto>(createDto);
        centroCusto.Id = Guid.NewGuid();
        centroCusto.Ativa = true;
        centroCusto.DataCriacao = DateTime.UtcNow;

        await _centroCustoRepository.AddAsync(centroCusto);
        await _centroCustoRepository.SaveChangesAsync();

        // Recarregar com relacionamentos para DTO
        centroCusto = await _centroCustoRepository.GetByIdAsync(centroCusto.Id);

        _logger.LogInformation("Centro de custo criado com sucesso: {CentroCustoId}", centroCusto?.Id);

        var centroCustoDto = _mapper.Map<CentroCustoDto>(centroCusto!);
        return ServiceResult<CentroCustoDto>.SuccessResult(centroCustoDto);
    }

    public async Task<ServiceResult<CentroCustoDto>> UpdateAsync(Guid id, UpdateCentroCustoDto updateDto)
    {
        _logger.LogInformation("Atualizando centro de custo: {CentroCustoId}", id);

        var centroCusto = await _centroCustoRepository.GetByIdAsync(id);

        if (centroCusto == null)
        {
            _logger.LogWarning("Centro de custo não encontrado para atualização: {CentroCustoId}", id);
            return ServiceResult<CentroCustoDto>.ErrorResult("Centro de custo não encontrado");
        }

        // Configurar contexto para validação
        _updateDbValidator.SetContext(id, centroCusto.SubAgrupamentoId);

        // Validar dados usando FluentValidation
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<CentroCustoDto>.ValidationErrorResult(errors);
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, centroCusto);
        centroCusto.DataUltimaAlteracao = DateTime.UtcNow;

        _centroCustoRepository.Update(centroCusto);
        await _centroCustoRepository.SaveChangesAsync();

        _logger.LogInformation("Centro de custo atualizado com sucesso: {CentroCustoId}", id);

        var centroCustoDto = _mapper.Map<CentroCustoDto>(centroCusto);
        return ServiceResult<CentroCustoDto>.SuccessResult(centroCustoDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando centro de custo: {CentroCustoId}", id);

        var centroCusto = await _centroCustoRepository.GetByIdWithCategoriasAsync(id);

        if (centroCusto == null)
        {
            _logger.LogWarning("Centro de custo não encontrado para desativação: {CentroCustoId}", id);
            return ServiceResult<bool>.ErrorResult("Centro de custo não encontrado");
        }

        // Verificar se centro de custo tem categorias ativas
        var categoriasAtivas = centroCusto.Categorias.Count(c => c.Ativa);
        if (categoriasAtivas > 0)
        {
            _logger.LogWarning("Tentativa de desativar centro de custo com categorias ativas: {CentroCustoId}, Categorias: {Count}", id, categoriasAtivas);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar centro de custo com {categoriasAtivas} categoria(s) ativa(s)");
        }

        _centroCustoRepository.SoftDelete(centroCusto);
        await _centroCustoRepository.SaveChangesAsync();

        _logger.LogInformation("Centro de custo desativado com sucesso: {CentroCustoId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCodigoInSubAgrupamentoAsync(Guid subAgrupamentoId, string codigo, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _centroCustoRepository.ExistsByCodigoInSubAgrupamentoAsync(subAgrupamentoId, codigo, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInSubAgrupamentoAsync(Guid subAgrupamentoId, string nome, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _centroCustoRepository.ExistsByNomeInSubAgrupamentoAsync(subAgrupamentoId, nome, excludeId));
    }

}