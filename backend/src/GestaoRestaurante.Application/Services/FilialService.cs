using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Caching;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.Services;

public class FilialService : IFilialService
{
    private readonly IFilialRepository _filialRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerformanceProfiler _profiler;
    private readonly IApplicationMetrics _metrics;
    private readonly ICacheService _cache;
    private readonly ILogger<FilialService> _logger;

    public FilialService(
        IFilialRepository filialRepository,
        IEmpresaRepository empresaRepository,
        IPerformanceProfiler profiler,
        IApplicationMetrics metrics,
        ICacheService cache,
        ILogger<FilialService> logger)
    {
        _filialRepository = filialRepository;
        _empresaRepository = empresaRepository;
        _profiler = profiler;
        _metrics = metrics;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<FilialDto>>> GetAllAsync()
    {
        using var measurement = _profiler.StartMeasurement("filial.get_all");
        _metrics.IncrementCounter("filial.operations", new Dictionary<string, string> { ["operation"] = "get_all" });

        try
        {
            // Tentar cache primeiro
            var cachedResult = await _cache.GetOrSetAsync(
                "filial:all:active",
                async () =>
                {
                    _logger.LogInformation("Buscando todas as filiais ativas");
                    var filiais = await _filialRepository.GetAllAsync().ConfigureAwait(false);
                    return filiais.Select(MapToDto).ToList();
                },
                TimeSpan.FromMinutes(10)
            ).ConfigureAwait(false);

            _logger.LogInformation("Encontradas {Count} filiais", cachedResult.Count());
            _metrics.SetGauge("filial.total_count", cachedResult.Count());
            
            return ServiceResult<IEnumerable<FilialDto>>.SuccessResult(cachedResult);
        }
        catch (Exception ex)
        {
            _metrics.IncrementCounter("filial.errors", new Dictionary<string, string> { ["operation"] = "get_all" });
            _logger.LogError(ex, "Erro ao buscar filiais");
            throw;
        }
    }

    public async Task<ServiceResult<FilialDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando filial com ID: {FilialId}", id);

        var filial = await _filialRepository.GetByIdAsync(id);

        if (filial == null)
        {
            _logger.LogWarning("Filial não encontrada: {FilialId}", id);
            return ServiceResult<FilialDto>.ErrorResult("Filial não encontrada");
        }

        var filialDto = MapToDto(filial);
        return ServiceResult<FilialDto>.SuccessResult(filialDto);
    }

    public async Task<ServiceResult<IEnumerable<FilialDto>>> GetByEmpresaAsync(Guid empresaId)
    {
        _logger.LogInformation("Buscando filiais da empresa: {EmpresaId}", empresaId);

        var filiais = await _filialRepository.GetByEmpresaIdAsync(empresaId);

        var filiaisDto = filiais.Select(MapToDto).ToList();

        _logger.LogInformation("Encontradas {Count} filiais para empresa {EmpresaId}", filiaisDto.Count, empresaId);
        return ServiceResult<IEnumerable<FilialDto>>.SuccessResult(filiaisDto);
    }

    public async Task<ServiceResult<FilialDto>> CreateAsync(CreateFilialDto createDto)
    {
        _logger.LogInformation("Criando nova filial: {Nome} para empresa {EmpresaId}", createDto.Nome, createDto.EmpresaId);

        // Validar dados
        var validationResult = await ValidateCreateAsync(createDto);
        if (!validationResult.Success)
        {
            return ServiceResult<FilialDto>.ValidationErrorResult(validationResult.ValidationErrors);
        }

        var filial = new Filial
        {
            Id = Guid.NewGuid(),
            EmpresaId = createDto.EmpresaId,
            Nome = createDto.Nome.Trim(),
            CnpjFilial = createDto.Cnpj?.Trim(),
            Email = createDto.Email?.Trim().ToLowerInvariant(),
            Telefone = createDto.Telefone?.Trim(),
            EnderecoFilial = ConvertToEndereco(createDto.Endereco),
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await _filialRepository.AddAsync(filial);
        await _filialRepository.SaveChangesAsync();

        // Recarregar com empresa para DTO
        var filialCompleta = await _filialRepository.GetByIdAsync(filial.Id);

        _logger.LogInformation("Filial criada com sucesso: {FilialId}", filial.Id);

        var filialDto = MapToDto(filialCompleta!);
        return ServiceResult<FilialDto>.SuccessResult(filialDto);
    }

    public async Task<ServiceResult<FilialDto>> UpdateAsync(Guid id, UpdateFilialDto updateDto)
    {
        _logger.LogInformation("Atualizando filial: {FilialId}", id);

        var filial = await _filialRepository.GetByIdAsync(id);

        if (filial == null || !filial.Ativa)
        {
            _logger.LogWarning("Filial não encontrada para atualização: {FilialId}", id);
            return ServiceResult<FilialDto>.ErrorResult("Filial não encontrada");
        }

        // Validar dados
        var validationResult = await ValidateUpdateAsync(id, filial.EmpresaId, updateDto);
        if (!validationResult.Success)
        {
            return ServiceResult<FilialDto>.ValidationErrorResult(validationResult.ValidationErrors);
        }

        // Atualizar dados
        filial.Nome = updateDto.Nome.Trim();
        filial.CnpjFilial = updateDto.Cnpj?.Trim();
        filial.Email = updateDto.Email?.Trim().ToLowerInvariant();
        filial.Telefone = updateDto.Telefone?.Trim();
        filial.EnderecoFilial = ConvertToEndereco(updateDto.Endereco);

        _filialRepository.Update(filial);
        await _filialRepository.SaveChangesAsync();

        _logger.LogInformation("Filial atualizada com sucesso: {FilialId}", id);

        var filialDto = MapToDto(filial);
        return ServiceResult<FilialDto>.SuccessResult(filialDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando filial: {FilialId}", id);

        var filial = await _filialRepository.GetByIdAsync(id);

        if (filial == null)
        {
            _logger.LogWarning("Filial não encontrada para desativação: {FilialId}", id);
            return ServiceResult<bool>.ErrorResult("Filial não encontrada");
        }

        // Verificar se é a única filial da empresa
        var isUnicaResult = await IsUnicaFilialDaEmpresaAsync(id);
        if (isUnicaResult.Data)
        {
            _logger.LogWarning("Tentativa de desativar a única filial ativa da empresa: {FilialId}", id);
            return ServiceResult<bool>.ErrorResult("Não é possível desativar a única filial ativa da empresa");
        }

        filial.Ativa = false;
        _filialRepository.Update(filial);
        await _filialRepository.SaveChangesAsync();

        _logger.LogInformation("Filial desativada com sucesso: {FilialId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null)
    {
        if (string.IsNullOrEmpty(cnpj))
        {
            return ServiceResult<bool>.SuccessResult(false);
        }

        var exists = await _filialRepository.ExistsByCnpjAsync(cnpj, excludeId);
        return ServiceResult<bool>.SuccessResult(exists);
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null)
    {
        var exists = await _filialRepository.ExistsByNomeInEmpresaAsync(empresaId, nome, excludeId);
        return ServiceResult<bool>.SuccessResult(exists);
    }

    public async Task<ServiceResult<bool>> IsUnicaFilialDaEmpresaAsync(Guid filialId)
    {
        var isUnica = await _filialRepository.IsUnicaFilialDaEmpresaAsync(filialId);
        return ServiceResult<bool>.SuccessResult(isUnica);
    }

    private async Task<ServiceResult<object>> ValidateCreateAsync(CreateFilialDto createDto)
    {
        var errors = new List<string>();

        // Validar Data Annotations
        var validationContext = new ValidationContext(createDto);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(createDto, validationContext, validationResults, true))
        {
            errors.AddRange(validationResults.Select(v => v.ErrorMessage ?? "Erro de validação"));
        }

        // Verificar se empresa existe e está ativa
        var empresa = await _empresaRepository.GetByIdAsync(createDto.EmpresaId);
        var empresaExists = empresa != null;

        if (!empresaExists)
        {
            errors.Add($"Empresa com ID {createDto.EmpresaId} não encontrada ou inativa");
        }

        // Validar unicidade nome na empresa
        var nomeExists = await ExistsByNomeInEmpresaAsync(createDto.EmpresaId, createDto.Nome);
        if (nomeExists.Data)
        {
            errors.Add($"Já existe uma filial com nome '{createDto.Nome}' nesta empresa");
        }

        // Validar unicidade CNPJ (se informado)
        if (!string.IsNullOrEmpty(createDto.Cnpj))
        {
            var cnpjExists = await ExistsByCnpjAsync(createDto.Cnpj);
            if (cnpjExists.Data)
            {
                errors.Add($"Já existe uma filial com CNPJ {createDto.Cnpj}");
            }
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

        return errors.Count != 0
            ? ServiceResult<object>.ValidationErrorResult(errors)
            : ServiceResult<object>.SuccessResult(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    }

    private async Task<ServiceResult<object>> ValidateUpdateAsync(Guid id, Guid empresaId, UpdateFilialDto updateDto)
    {
        var errors = new List<string>();

        // Validar Data Annotations
        var validationContext = new ValidationContext(updateDto);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(updateDto, validationContext, validationResults, true))
        {
            errors.AddRange(validationResults.Select(v => v.ErrorMessage ?? "Erro de validação"));
        }

        // Validar unicidade nome na empresa
        var nomeExists = await ExistsByNomeInEmpresaAsync(empresaId, updateDto.Nome, id);
        if (nomeExists.Data)
        {
            errors.Add($"Já existe outra filial com nome '{updateDto.Nome}' nesta empresa");
        }

        // Validar unicidade CNPJ (se informado)
        if (!string.IsNullOrEmpty(updateDto.Cnpj))
        {
            var cnpjExists = await ExistsByCnpjAsync(updateDto.Cnpj, id);
            if (cnpjExists.Data)
            {
                errors.Add($"Já existe outra filial com CNPJ {updateDto.Cnpj}");
            }
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

        return errors.Count != 0
            ? ServiceResult<object>.ValidationErrorResult(errors)
            : ServiceResult<object>.SuccessResult(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    }

    private static FilialDto MapToDto(Filial filial)
    {
        return new FilialDto
        {
            Id = filial.Id,
            EmpresaId = filial.EmpresaId,
            Nome = filial.Nome,
            Cnpj = filial.CnpjFilial,
            Email = filial.Email,
            Telefone = filial.Telefone,
            Endereco = ConvertToEnderecoDto(filial.EnderecoFilial),
            Ativa = filial.Ativa,
            DataCriacao = filial.DataCriacao,
            EmpresaNome = filial.Empresa?.NomeFantasia ?? string.Empty
        };
    }

    private static Endereco? ConvertToEndereco(EnderecoDto? enderecoDto)
    {
        if (enderecoDto == null) return null;
        
        return new Endereco(
            enderecoDto.Logradouro ?? string.Empty,
            enderecoDto.Numero,
            enderecoDto.Complemento,
            enderecoDto.Cep ?? string.Empty,
            enderecoDto.Bairro ?? string.Empty,
            enderecoDto.Cidade ?? string.Empty,
            enderecoDto.Estado ?? string.Empty
        );
    }

    private static EnderecoDto? ConvertToEnderecoDto(Endereco? endereco)
    {
        if (endereco == null) return null;

#pragma warning disable CS8601 // Possible null reference assignment.

        return new EnderecoDto
        {
            Logradouro = endereco.Logradouro,
            Numero = endereco.Numero,
            Complemento = endereco.Complemento,
            Cep = endereco.Cep,
            Bairro = endereco.Bairro,
            Cidade = endereco.Cidade,
            Estado = endereco.Estado
        };
#pragma warning restore CS8601 // Possible null reference assignment.

    }
}