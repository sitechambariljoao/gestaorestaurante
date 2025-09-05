using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Services;

public class AgrupamentoService : IAgrupamentoService
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IMapper _mapper;
    private readonly CreateAgrupamentoDbValidator _createDbValidator;
    private readonly UpdateAgrupamentoDbValidator _updateDbValidator;
    private readonly ILogger<AgrupamentoService> _logger;

    public AgrupamentoService(
        IAgrupamentoRepository agrupamentoRepository,
        IEmpresaRepository empresaRepository,
        IMapper mapper,
        CreateAgrupamentoDbValidator createDbValidator,
        UpdateAgrupamentoDbValidator updateDbValidator,
        ILogger<AgrupamentoService> logger)
    {
        _agrupamentoRepository = agrupamentoRepository;
        _empresaRepository = empresaRepository;
        _mapper = mapper;
        _createDbValidator = createDbValidator;
        _updateDbValidator = updateDbValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<AgrupamentoDto>>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os agrupamentos ativos");

        var agrupamentos = await _agrupamentoRepository.GetAllAsync();
        var agrupamentosDto = _mapper.Map<List<AgrupamentoDto>>(agrupamentos);

        _logger.LogInformation("Encontrados {Count} agrupamentos", agrupamentosDto.Count);
        return ServiceResult<IEnumerable<AgrupamentoDto>>.SuccessResult(agrupamentosDto);
    }

    public async Task<ServiceResult<AgrupamentoDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando agrupamento com ID: {AgrupamentoId}", id);

        var agrupamento = await _agrupamentoRepository.GetByIdAsync(id);

        if (agrupamento == null)
        {
            _logger.LogWarning("Agrupamento não encontrado: {AgrupamentoId}", id);
            return ServiceResult<AgrupamentoDto>.ErrorResult("Agrupamento não encontrado");
        }

        var agrupamentoDto = _mapper.Map<AgrupamentoDto>(agrupamento);
        return ServiceResult<AgrupamentoDto>.SuccessResult(agrupamentoDto);
    }
    
    public async Task<ServiceResult<IEnumerable<AgrupamentoDto>>> GetByFilialIdAsync(Guid filialId)
    {
        _logger.LogInformation("Buscando agrupamentos da filial: {FilialId}", filialId);

        var agrupamentos = await _agrupamentoRepository.GetByFilialIdAsync(filialId);
        var agrupamentosDto = _mapper.Map<List<AgrupamentoDto>>(agrupamentos);

        _logger.LogInformation("Encontrados {Count} agrupamentos para filial {FilialId}", agrupamentosDto.Count, filialId);
        return ServiceResult<IEnumerable<AgrupamentoDto>>.SuccessResult(agrupamentosDto);
    }

    public async Task<ServiceResult<AgrupamentoDto>> CreateAsync(CreateAgrupamentoDto createDto)
    {
        _logger.LogInformation("Criando novo agrupamento: {Nome} para empresa {EmpresaId}", createDto.Nome, createDto.FilialId);

        // Validar dados usando FluentValidation
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<AgrupamentoDto>.ValidationErrorResult(errors);
        }

        var agrupamento = _mapper.Map<Agrupamento>(createDto);
        agrupamento.Id = Guid.NewGuid();
        agrupamento.Ativa = true;
        agrupamento.DataCriacao = DateTime.UtcNow;

        await _agrupamentoRepository.AddAsync(agrupamento);
        await _agrupamentoRepository.SaveChangesAsync();

        // Recarregar com empresa para DTO
        agrupamento = await _agrupamentoRepository.GetByIdAsync(agrupamento.Id);

        _logger.LogInformation("Agrupamento criado com sucesso: {AgrupamentoId}", agrupamento?.Id);

        var agrupamentoDto = _mapper.Map<AgrupamentoDto>(agrupamento!);
        return ServiceResult<AgrupamentoDto>.SuccessResult(agrupamentoDto);
    }

    public async Task<ServiceResult<AgrupamentoDto>> UpdateAsync(Guid id, UpdateAgrupamentoDto updateDto)
    {
        _logger.LogInformation("Atualizando agrupamento: {AgrupamentoId}", id);

        var agrupamento = await _agrupamentoRepository.GetByIdAsync(id);

        if (agrupamento == null)
        {
            _logger.LogWarning("Agrupamento não encontrado para atualização: {AgrupamentoId}", id);
            return ServiceResult<AgrupamentoDto>.ErrorResult("Agrupamento não encontrado");
        }

        // Validar dados usando FluentValidation
        _updateDbValidator.SetContext(id, agrupamento.FilialId);
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<AgrupamentoDto>.ValidationErrorResult(errors);
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, agrupamento);

        _agrupamentoRepository.Update(agrupamento);
        await _agrupamentoRepository.SaveChangesAsync();

        _logger.LogInformation("Agrupamento atualizado com sucesso: {AgrupamentoId}", id);

        var agrupamentoDto = _mapper.Map<AgrupamentoDto>(agrupamento);
        return ServiceResult<AgrupamentoDto>.SuccessResult(agrupamentoDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando agrupamento: {AgrupamentoId}", id);

        var agrupamento = await _agrupamentoRepository.GetByIdWithSubAgrupamentosAsync(id);

        if (agrupamento == null)
        {
            _logger.LogWarning("Agrupamento não encontrado para desativação: {AgrupamentoId}", id);
            return ServiceResult<bool>.ErrorResult("Agrupamento não encontrado");
        }

        // Verificar se agrupamento tem sub-agrupamentos ativos
        var subAgrupamentosAtivos = agrupamento.SubAgrupamentos.Count(s => s.Ativa);
        if (subAgrupamentosAtivos > 0)
        {
            _logger.LogWarning("Tentativa de desativar agrupamento com sub-agrupamentos ativos: {AgrupamentoId}, SubAgrupamentos: {Count}", id, subAgrupamentosAtivos);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar agrupamento com {subAgrupamentosAtivos} sub-agrupamento(s) ativo(s)");
        }

        _agrupamentoRepository.SoftDelete(agrupamento);
        await _agrupamentoRepository.SaveChangesAsync();

        _logger.LogInformation("Agrupamento desativado com sucesso: {AgrupamentoId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCodigoInEmpresaAsync(Guid empresaId, string codigo, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _agrupamentoRepository.ExistsByCodigoInEmpresaAsync(empresaId, codigo, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _agrupamentoRepository.ExistsByNomeInEmpresaAsync(empresaId, nome, excludeId));
    }


}