using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Services;

public class SubAgrupamentoService(
    ISubAgrupamentoRepository subAgrupamentoRepository,
    IAgrupamentoRepository agrupamentoRepository,
    IMapper mapper,
    CreateSubAgrupamentoDbValidator createDbValidator,
    UpdateSubAgrupamentoDbValidator updateDbValidator,
    ILogger<SubAgrupamentoService> logger) : ISubAgrupamentoService
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository = subAgrupamentoRepository;
    private readonly IAgrupamentoRepository _agrupamentoRepository = agrupamentoRepository;
    private readonly IMapper _mapper = mapper;
    private readonly CreateSubAgrupamentoDbValidator _createDbValidator = createDbValidator;
    private readonly UpdateSubAgrupamentoDbValidator _updateDbValidator = updateDbValidator;
    private readonly ILogger<SubAgrupamentoService> _logger = logger;

    public async Task<ServiceResult<IEnumerable<SubAgrupamentoDto>>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os sub-agrupamentos ativos");

        var subAgrupamentos = await _subAgrupamentoRepository.GetAllAsync();
        var subAgrupamentosDto = _mapper.Map<List<SubAgrupamentoDto>>(subAgrupamentos);

        _logger.LogInformation("Encontrados {Count} sub-agrupamentos", subAgrupamentosDto.Count);
        return ServiceResult<IEnumerable<SubAgrupamentoDto>>.SuccessResult(subAgrupamentosDto);
    }

    public async Task<ServiceResult<SubAgrupamentoDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando sub-agrupamento com ID: {SubAgrupamentoId}", id);

        var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(id);

        if (subAgrupamento == null)
        {
            _logger.LogWarning("Sub-agrupamento não encontrado: {SubAgrupamentoId}", id);
            return ServiceResult<SubAgrupamentoDto>.ErrorResult("Sub-agrupamento não encontrado");
        }

        var subAgrupamentoDto = _mapper.Map<SubAgrupamentoDto>(subAgrupamento);
        return ServiceResult<SubAgrupamentoDto>.SuccessResult(subAgrupamentoDto);
    }

    public async Task<ServiceResult<IEnumerable<SubAgrupamentoDto>>> GetByAgrupamentoIdAsync(Guid agrupamentoId)
    {
        _logger.LogInformation("Buscando sub-agrupamentos do agrupamento: {AgrupamentoId}", agrupamentoId);

        var subAgrupamentos = await _subAgrupamentoRepository.GetByAgrupamentoIdAsync(agrupamentoId);
        var subAgrupamentosDto = _mapper.Map<List<SubAgrupamentoDto>>(subAgrupamentos);

        _logger.LogInformation("Encontrados {Count} sub-agrupamentos para agrupamento {AgrupamentoId}", subAgrupamentosDto.Count, agrupamentoId);
        return ServiceResult<IEnumerable<SubAgrupamentoDto>>.SuccessResult(subAgrupamentosDto);
    }

    public async Task<ServiceResult<SubAgrupamentoDto>> CreateAsync(CreateSubAgrupamentoDto createDto)
    {
        _logger.LogInformation("Criando novo sub-agrupamento: {Nome} para agrupamento {AgrupamentoId}", createDto.Nome, createDto.AgrupamentoId);

        // Validar dados usando FluentValidation
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<SubAgrupamentoDto>.ValidationErrorResult(errors);
        }

        // Verificar se agrupamento existe e está ativo
        var agrupamentoExists = await _agrupamentoRepository.ExistsAsync(createDto.AgrupamentoId);
        if (!agrupamentoExists)
        {
            return ServiceResult<SubAgrupamentoDto>.ValidationErrorResult(new List<string> { $"Agrupamento com ID {createDto.AgrupamentoId} não encontrado ou inativo" });
        }

        var subAgrupamento = _mapper.Map<SubAgrupamento>(createDto);
        subAgrupamento.Id = Guid.NewGuid();
        subAgrupamento.Ativa = true;
        subAgrupamento.DataCriacao = DateTime.UtcNow;

        await _subAgrupamentoRepository.AddAsync(subAgrupamento);
        await _subAgrupamentoRepository.SaveChangesAsync();

        // Recarregar com agrupamento para DTO
        subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(subAgrupamento.Id);

        _logger.LogInformation("Sub-agrupamento criado com sucesso: {SubAgrupamentoId}", subAgrupamento?.Id);

        var subAgrupamentoDto = _mapper.Map<SubAgrupamentoDto>(subAgrupamento!);
        return ServiceResult<SubAgrupamentoDto>.SuccessResult(subAgrupamentoDto);
    }

    public async Task<ServiceResult<SubAgrupamentoDto>> UpdateAsync(Guid id, UpdateSubAgrupamentoDto updateDto)
    {
        _logger.LogInformation("Atualizando sub-agrupamento: {SubAgrupamentoId}", id);

        var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(id);

        if (subAgrupamento == null)
        {
            _logger.LogWarning("Sub-agrupamento não encontrado para atualização: {SubAgrupamentoId}", id);
            return ServiceResult<SubAgrupamentoDto>.ErrorResult("Sub-agrupamento não encontrado");
        }

        // Configurar contexto para validação
        _updateDbValidator.SetContext(id, subAgrupamento.AgrupamentoId);

        // Validar dados usando FluentValidation
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<SubAgrupamentoDto>.ValidationErrorResult(errors);
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, subAgrupamento);
        subAgrupamento.DataUltimaAlteracao = DateTime.UtcNow;

        _subAgrupamentoRepository.Update(subAgrupamento);
        await _subAgrupamentoRepository.SaveChangesAsync();

        _logger.LogInformation("Sub-agrupamento atualizado com sucesso: {SubAgrupamentoId}", id);

        var subAgrupamentoDto = _mapper.Map<SubAgrupamentoDto>(subAgrupamento);
        return ServiceResult<SubAgrupamentoDto>.SuccessResult(subAgrupamentoDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando sub-agrupamento: {SubAgrupamentoId}", id);

        var subAgrupamento = await _subAgrupamentoRepository.GetByIdWithCentrosCustoAsync(id);

        if (subAgrupamento == null)
        {
            _logger.LogWarning("Sub-agrupamento não encontrado para desativação: {SubAgrupamentoId}", id);
            return ServiceResult<bool>.ErrorResult("Sub-agrupamento não encontrado");
        }

        // Verificar se sub-agrupamento tem centros de custo ativos
        var centrosCustoAtivos = subAgrupamento.CentrosCusto.Count(c => c.Ativa);
        if (centrosCustoAtivos > 0)
        {
            _logger.LogWarning("Tentativa de desativar sub-agrupamento com centros de custo ativos: {SubAgrupamentoId}, CentrosCusto: {Count}", id, centrosCustoAtivos);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar sub-agrupamento com {centrosCustoAtivos} centro(s) de custo ativo(s)");
        }

        _subAgrupamentoRepository.SoftDelete(subAgrupamento);
        await _subAgrupamentoRepository.SaveChangesAsync();

        _logger.LogInformation("Sub-agrupamento desativado com sucesso: {SubAgrupamentoId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCodigoInAgrupamentoAsync(Guid agrupamentoId, string codigo, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _subAgrupamentoRepository.ExistsByCodigoInAgrupamentoAsync(agrupamentoId, codigo, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInAgrupamentoAsync(Guid agrupamentoId, string nome, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _subAgrupamentoRepository.ExistsByNomeInAgrupamentoAsync(agrupamentoId, nome, excludeId));
    }

}