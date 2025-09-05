using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly IMapper _mapper;
    private readonly CreateCategoriaDbValidator _createDbValidator;
    private readonly UpdateCategoriaDbValidator _updateDbValidator;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(
        ICategoriaRepository categoriaRepository,
        ICentroCustoRepository centroCustoRepository,
        IMapper mapper,
        CreateCategoriaDbValidator createDbValidator,
        UpdateCategoriaDbValidator updateDbValidator,
        ILogger<CategoriaService> logger)
    {
        _categoriaRepository = categoriaRepository;
        _centroCustoRepository = centroCustoRepository;
        _mapper = mapper;
        _createDbValidator = createDbValidator;
        _updateDbValidator = updateDbValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<CategoriaDto>>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todas as categorias ativas");

        var categorias = await _categoriaRepository.GetAllAsync();
        var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

        _logger.LogInformation("Encontradas {Count} categorias", categoriasDto.Count);
        return ServiceResult<IEnumerable<CategoriaDto>>.SuccessResult(categoriasDto);
    }

    public async Task<ServiceResult<CategoriaDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando categoria com ID: {CategoriaId}", id);

        var categoria = await _categoriaRepository.GetByIdAsync(id);

        if (categoria == null)
        {
            _logger.LogWarning("Categoria não encontrada: {CategoriaId}", id);
            return ServiceResult<CategoriaDto>.ErrorResult("Categoria não encontrada");
        }

        var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
        return ServiceResult<CategoriaDto>.SuccessResult(categoriaDto);
    }

    public async Task<ServiceResult<IEnumerable<CategoriaDto>>> GetByCentroCustoIdAsync(Guid centroCustoId)
    {
        _logger.LogInformation("Buscando categorias do centro de custo: {CentroCustoId}", centroCustoId);

        var categorias = await _categoriaRepository.GetByCentroCustoIdAsync(centroCustoId);
        var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

        _logger.LogInformation("Encontradas {Count} categorias para centro de custo {CentroCustoId}", categoriasDto.Count, centroCustoId);
        return ServiceResult<IEnumerable<CategoriaDto>>.SuccessResult(categoriasDto);
    }

    public async Task<ServiceResult<IEnumerable<CategoriaDto>>> GetByNivelAsync(int nivel)
    {
        _logger.LogInformation("Buscando categorias do nível: {Nivel}", nivel);

        if (nivel < 1 || nivel > 3)
        {
            return ServiceResult<IEnumerable<CategoriaDto>>.ErrorResult("Nível deve ser entre 1 e 3");
        }

        var categorias = await _categoriaRepository.GetByNivelAsync(nivel);
        var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

        _logger.LogInformation("Encontradas {Count} categorias para nível {Nivel}", categoriasDto.Count, nivel);
        return ServiceResult<IEnumerable<CategoriaDto>>.SuccessResult(categoriasDto);
    }

    public async Task<ServiceResult<IEnumerable<CategoriaDto>>> GetFilhasByPaiIdAsync(Guid categoriaPaiId)
    {
        _logger.LogInformation("Buscando categorias filhas da categoria pai: {CategoriaPaiId}", categoriaPaiId);

        var categorias = await _categoriaRepository.GetFilhasByPaiIdAsync(categoriaPaiId);
        var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

        _logger.LogInformation("Encontradas {Count} categorias filhas para categoria pai {CategoriaPaiId}", categoriasDto.Count, categoriaPaiId);
        return ServiceResult<IEnumerable<CategoriaDto>>.SuccessResult(categoriasDto);
    }

    public async Task<ServiceResult<IEnumerable<CategoriaDto>>> GetHierarquiaByCentroCustoAsync(Guid centroCustoId)
    {
        _logger.LogInformation("Buscando hierarquia de categorias do centro de custo: {CentroCustoId}", centroCustoId);

        var categorias = await _categoriaRepository.GetHierarquiaByCentroCustoAsync(centroCustoId);
        var categoriasDto = BuildHierarchy(_mapper.Map<List<CategoriaDto>>(categorias));

        _logger.LogInformation("Encontradas {Count} categorias na hierarquia para centro de custo {CentroCustoId}", categoriasDto.Count, centroCustoId);
        return ServiceResult<IEnumerable<CategoriaDto>>.SuccessResult(categoriasDto);
    }

    public async Task<ServiceResult<CategoriaDto>> CreateAsync(CreateCategoriaDto createDto)
    {
        _logger.LogInformation("Criando nova categoria: {Nome} para centro de custo {CentroCustoId}", createDto.Nome, createDto.CentroCustoId);

        // Validar dados usando FluentValidation
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<CategoriaDto>.ValidationErrorResult(errors);
        }

        // Verificar se centro de custo existe e está ativo
        var centroCustoExists = await _centroCustoRepository.ExistsAsync(createDto.CentroCustoId);
        if (!centroCustoExists)
        {
            return ServiceResult<CategoriaDto>.ValidationErrorResult(new List<string> { $"Centro de custo com ID {createDto.CentroCustoId} não encontrado ou inativo" });
        }

        // Validar nível e categoria pai
        if (createDto.Nivel > 1 && createDto.CategoriaPaiId == null)
        {
            return ServiceResult<CategoriaDto>.ValidationErrorResult(new List<string> { "Categorias de nível 2 ou 3 devem ter uma categoria pai definida" });
        }

        if (createDto.Nivel == 1 && createDto.CategoriaPaiId != null)
        {
            return ServiceResult<CategoriaDto>.ValidationErrorResult(new List<string> { "Categorias de nível 1 não podem ter categoria pai" });
        }

        if (createDto.CategoriaPaiId.HasValue)
        {
            var categoriaPai = await _categoriaRepository.GetByIdAsync(createDto.CategoriaPaiId.Value);
            if (categoriaPai == null)
            {
                return ServiceResult<CategoriaDto>.ValidationErrorResult(new List<string> { "Categoria pai não encontrada ou inativa" });
            }

            // Validar hierarquia de níveis
            if (categoriaPai.Nivel >= createDto.Nivel)
            {
                return ServiceResult<CategoriaDto>.ValidationErrorResult(new List<string> { "A categoria pai deve ter nível inferior à categoria sendo criada" });
            }
        }

        var categoria = _mapper.Map<Categoria>(createDto);
        categoria.Id = Guid.NewGuid();
        categoria.Ativa = true;
        categoria.DataCriacao = DateTime.UtcNow;

        await _categoriaRepository.AddAsync(categoria);
        await _categoriaRepository.SaveChangesAsync();

        // Recarregar com relacionamentos para DTO
        categoria = await _categoriaRepository.GetByIdAsync(categoria.Id);

        _logger.LogInformation("Categoria criada com sucesso: {CategoriaId}", categoria?.Id);

        var categoriaDto = _mapper.Map<CategoriaDto>(categoria!);
        return ServiceResult<CategoriaDto>.SuccessResult(categoriaDto);
    }

    public async Task<ServiceResult<CategoriaDto>> UpdateAsync(Guid id, UpdateCategoriaDto updateDto)
    {
        _logger.LogInformation("Atualizando categoria: {CategoriaId}", id);

        var categoria = await _categoriaRepository.GetByIdAsync(id);

        if (categoria == null)
        {
            _logger.LogWarning("Categoria não encontrada para atualização: {CategoriaId}", id);
            return ServiceResult<CategoriaDto>.ErrorResult("Categoria não encontrada");
        }

        // Configurar contexto para validação
        _updateDbValidator.SetContext(id, categoria.CentroCustoId);

        // Validar dados usando FluentValidation
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<CategoriaDto>.ValidationErrorResult(errors);
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, categoria);
        categoria.DataUltimaAlteracao = DateTime.UtcNow;

        _categoriaRepository.Update(categoria);
        await _categoriaRepository.SaveChangesAsync();

        _logger.LogInformation("Categoria atualizada com sucesso: {CategoriaId}", id);

        var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
        return ServiceResult<CategoriaDto>.SuccessResult(categoriaDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando categoria: {CategoriaId}", id);

        var categoria = await _categoriaRepository.GetByIdWithFilhasAsync(id);

        if (categoria == null)
        {
            _logger.LogWarning("Categoria não encontrada para desativação: {CategoriaId}", id);
            return ServiceResult<bool>.ErrorResult("Categoria não encontrada");
        }

        // Verificar se categoria tem categorias filhas ativas
        var categoriasFilhasAtivas = categoria.CategoriasFilhas.Count(c => c.Ativa);
        if (categoriasFilhasAtivas > 0)
        {
            _logger.LogWarning("Tentativa de desativar categoria com categorias filhas ativas: {CategoriaId}, CategoriasFilhas: {Count}", id, categoriasFilhasAtivas);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar categoria com {categoriasFilhasAtivas} categoria(s) filha(s) ativa(s)");
        }

        // Verificar se categoria tem produtos ativos
        var produtosAtivos = categoria.Produtos?.Count(p => p.Ativa) ?? 0;
        if (produtosAtivos > 0)
        {
            _logger.LogWarning("Tentativa de desativar categoria com produtos ativos: {CategoriaId}, Produtos: {Count}", id, produtosAtivos);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar categoria com {produtosAtivos} produto(s) ativo(s)");
        }

        _categoriaRepository.SoftDelete(categoria);
        await _categoriaRepository.SaveChangesAsync();

        _logger.LogInformation("Categoria desativada com sucesso: {CategoriaId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCodigoInCentroCustoAsync(Guid centroCustoId, string codigo, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _categoriaRepository.ExistsByCodigoInCentroCustoAsync(centroCustoId, codigo, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInCentroCustoAsync(Guid centroCustoId, string nome, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _categoriaRepository.ExistsByNomeInCentroCustoAsync(centroCustoId, nome, excludeId));
    }


    private List<CategoriaDto> BuildHierarchy(List<CategoriaDto> categorias)
    {
        var categoriasNivel1 = categorias.Where(c => c.Nivel == 1).ToList();
        
        foreach (var categoria in categoriasNivel1)
        {
            BuildChildren(categoria, categorias);
        }
        
        return categoriasNivel1;
    }

    private void BuildChildren(CategoriaDto categoria, List<CategoriaDto> todasCategorias)
    {
        var filhas = todasCategorias.Where(c => c.CategoriaPaiId == categoria.Id).ToList();
        categoria.CategoriasFilhas = filhas;
        
        foreach (var filha in filhas)
        {
            BuildChildren(filha, todasCategorias);
        }
    }

}