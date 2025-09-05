using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Services;

namespace GestaoRestaurante.Application.Features.Produtos.Commands.CreateProduto;

/// <summary>
/// Handler para o comando de criação de produto
/// </summary>
public sealed class CreateProdutoCommandHandler : ICommandHandler<CreateProdutoCommand, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoDomainService _produtoDomainService;
    private readonly CreateProdutoDbValidator _dbValidator;
    private readonly IMapper _mapper;

    public CreateProdutoCommandHandler(
        IProdutoRepository produtoRepository,
        IProdutoDomainService produtoDomainService,
        CreateProdutoDbValidator dbValidator,
        IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _produtoDomainService = produtoDomainService;
        _dbValidator = dbValidator;
        _mapper = mapper;
    }

    public async Task<Result<ProdutoDto>> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar regras de domínio
            var domainValidationResult = await _produtoDomainService.ValidateProdutoCreationAsync(
                request.Codigo, request.Nome, request.CategoriaId);
            
            if (!domainValidationResult.IsSuccess)
                return Result<ProdutoDto>.Failure(domainValidationResult.Errors);

            // Validar preço
            var priceValidationResult = _produtoDomainService.ValidatePriceChange(0, request.Preco);
            if (!priceValidationResult.IsSuccess)
                return Result<ProdutoDto>.Failure(priceValidationResult.ErrorMessage);

            // Validar regras de banco de dados
            var dbValidationResult = await _dbValidator.ValidateAsync(
                new CreateProdutoDto
                {
                    CategoriaId = request.CategoriaId,
                    Codigo = request.Codigo,
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    Preco = request.Preco,
                    UnidadeMedida = request.UnidadeMedida,
                    ProdutoVenda = request.ProdutoVenda,
                    ProdutoEstoque = request.ProdutoEstoque
                });

            if (!dbValidationResult.IsValid)
                return Result<ProdutoDto>.Failure(dbValidationResult.Errors.Select(e => e.ErrorMessage));

            // Criar produto
            var produto = new Produto(
                request.CategoriaId,
                request.Codigo,
                request.Nome,
                request.Preco,
                request.UnidadeMedida,
                request.Descricao,
                request.ProdutoVenda,
                request.ProdutoEstoque
            );

            // Validar invariantes do aggregate
            produto.ValidateInvariants();

            // Persistir
            await _produtoRepository.AddAsync(produto);
            await _produtoRepository.SaveChangesAsync();

            // Mapear para DTO
            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            return Result<ProdutoDto>.Success(produtoDto);
        }
        catch (Exception ex)
        {
            return Result<ProdutoDto>.Failure($"Erro ao criar produto: {ex.Message}");
        }
    }
}