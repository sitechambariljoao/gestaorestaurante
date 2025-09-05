using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly CreateProdutoDbValidator _createDbValidator;
    private readonly UpdateProdutoDbValidator _updateDbValidator;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IMapper mapper,
        CreateProdutoDbValidator createDbValidator,
        UpdateProdutoDbValidator updateDbValidator,
        ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
        _createDbValidator = createDbValidator;
        _updateDbValidator = updateDbValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ProdutoDto>>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os produtos ativos");

        var produtos = await _produtoRepository.GetAllAsync();
        var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

        _logger.LogInformation("Encontrados {Count} produtos", produtosDto.Count);
        return ServiceResult<IEnumerable<ProdutoDto>>.SuccessResult(produtosDto);
    }

    public async Task<ServiceResult<ProdutoDto>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando produto com ID: {ProdutoId}", id);

        var produto = await _produtoRepository.GetByIdAsync(id);

        if (produto == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProdutoId}", id);
            return ServiceResult<ProdutoDto>.ErrorResult("Produto não encontrado");
        }

        var produtoDto = _mapper.Map<ProdutoDto>(produto);
        return ServiceResult<ProdutoDto>.SuccessResult(produtoDto);
    }

    public async Task<ServiceResult<IEnumerable<ProdutoDto>>> GetByCategoriaIdAsync(Guid categoriaId)
    {
        _logger.LogInformation("Buscando produtos da categoria: {CategoriaId}", categoriaId);

        var produtos = await _produtoRepository.GetByCategoriaIdAsync(categoriaId);
        var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

        _logger.LogInformation("Encontrados {Count} produtos para categoria {CategoriaId}", produtosDto.Count, categoriaId);
        return ServiceResult<IEnumerable<ProdutoDto>>.SuccessResult(produtosDto);
    }

    public async Task<ServiceResult<IEnumerable<ProdutoDto>>> GetProdutosVendaAsync()
    {
        _logger.LogInformation("Buscando produtos de venda");

        var produtos = await _produtoRepository.GetProdutosVendaAsync();
        var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

        _logger.LogInformation("Encontrados {Count} produtos de venda", produtosDto.Count);
        return ServiceResult<IEnumerable<ProdutoDto>>.SuccessResult(produtosDto);
    }

    public async Task<ServiceResult<IEnumerable<ProdutoDto>>> GetProdutosEstoqueAsync()
    {
        _logger.LogInformation("Buscando produtos de estoque");

        var produtos = await _produtoRepository.GetProdutosEstoqueAsync();
        var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

        _logger.LogInformation("Encontrados {Count} produtos de estoque", produtosDto.Count);
        return ServiceResult<IEnumerable<ProdutoDto>>.SuccessResult(produtosDto);
    }

    public async Task<ServiceResult<ProdutoDto>> CreateAsync(CreateProdutoDto createDto)
    {
        _logger.LogInformation("Criando novo produto: {Nome} para categoria {CategoriaId}", createDto.Nome, createDto.CategoriaId);

        // Validar dados usando FluentValidation
        var validationResult = await _createDbValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<ProdutoDto>.ValidationErrorResult(errors);
        }

        // Verificar se categoria existe e está ativa
        var categoria = await _categoriaRepository.GetByIdAsync(createDto.CategoriaId);
        if (categoria == null)
        {
            return ServiceResult<ProdutoDto>.ValidationErrorResult(new List<string> { $"Categoria com ID {createDto.CategoriaId} não encontrada ou inativa" });
        }

        // Validar se categoria é de nível 3 (só pode ter produtos)
        if (categoria.Nivel != 3)
        {
            return ServiceResult<ProdutoDto>.ValidationErrorResult(new List<string> { "Produtos só podem ser vinculados a categorias de nível 3" });
        }

        // Validar que pelo menos um tipo de produto está marcado
        if (!createDto.ProdutoVenda && !createDto.ProdutoEstoque)
        {
            return ServiceResult<ProdutoDto>.ValidationErrorResult(new List<string> { "Produto deve ser marcado como 'Produto de Venda' ou 'Produto de Estoque' (ou ambos)" });
        }

        var produto = _mapper.Map<Produto>(createDto);
        produto.Id = Guid.NewGuid();
        produto.Ativa = true;
        produto.DataCriacao = DateTime.UtcNow;

        await _produtoRepository.AddAsync(produto);
        await _produtoRepository.SaveChangesAsync();

        // Recarregar com relacionamentos para DTO
        produto = await _produtoRepository.GetByIdAsync(produto.Id);

        _logger.LogInformation("Produto criado com sucesso: {ProdutoId}", produto?.Id);

        var produtoDto = _mapper.Map<ProdutoDto>(produto!);
        return ServiceResult<ProdutoDto>.SuccessResult(produtoDto);
    }

    public async Task<ServiceResult<ProdutoDto>> UpdateAsync(Guid id, UpdateProdutoDto updateDto)
    {
        _logger.LogInformation("Atualizando produto: {ProdutoId}", id);

        var produto = await _produtoRepository.GetByIdAsync(id);

        if (produto == null)
        {
            _logger.LogWarning("Produto não encontrado para atualização: {ProdutoId}", id);
            return ServiceResult<ProdutoDto>.ErrorResult("Produto não encontrado");
        }

        // Configurar contexto para validação
        _updateDbValidator.SetContext(id, produto.CategoriaId);

        // Validar dados usando FluentValidation
        var validationResult = await _updateDbValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResult<ProdutoDto>.ValidationErrorResult(errors);
        }

        // Validar que pelo menos um tipo de produto está marcado
        if (!updateDto.ProdutoVenda && !updateDto.ProdutoEstoque)
        {
            return ServiceResult<ProdutoDto>.ValidationErrorResult(new List<string> { "Produto deve ser marcado como 'Produto de Venda' ou 'Produto de Estoque' (ou ambos)" });
        }

        // Atualizar dados usando AutoMapper
        _mapper.Map(updateDto, produto);
        produto.DataUltimaAlteracao = DateTime.UtcNow;

        _produtoRepository.Update(produto);
        await _produtoRepository.SaveChangesAsync();

        _logger.LogInformation("Produto atualizado com sucesso: {ProdutoId}", id);

        var produtoDto = _mapper.Map<ProdutoDto>(produto);
        return ServiceResult<ProdutoDto>.SuccessResult(produtoDto);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Desativando produto: {ProdutoId}", id);

        var produto = await _produtoRepository.GetByIdWithIngredientesAsync(id);

        if (produto == null)
        {
            _logger.LogWarning("Produto não encontrado para desativação: {ProdutoId}", id);
            return ServiceResult<bool>.ErrorResult("Produto não encontrado");
        }

        // Verificar se produto tem ingredientes vinculados
        var ingredientesVinculados = produto.Ingredientes?.Count ?? 0;
        if (ingredientesVinculados > 0)
        {
            _logger.LogWarning("Tentativa de desativar produto com ingredientes vinculados: {ProdutoId}, Ingredientes: {Count}", id, ingredientesVinculados);
            return ServiceResult<bool>.ErrorResult($"Não é possível desativar produto com {ingredientesVinculados} ingrediente(s) vinculado(s)");
        }

        _produtoRepository.SoftDelete(produto);
        await _produtoRepository.SaveChangesAsync();

        _logger.LogInformation("Produto desativado com sucesso: {ProdutoId}", id);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> ExistsByCodigoAsync(string codigo, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _produtoRepository.ExistsByCodigoAsync(codigo, excludeId));
    }

    public async Task<ServiceResult<bool>> ExistsByNomeInCategoriaAsync(Guid categoriaId, string nome, Guid? excludeId = null)
    {
        return ServiceResult<bool>.SuccessResult(await _produtoRepository.ExistsByNomeInCategoriaAsync(categoriaId, nome, excludeId));
    }

}