using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;

namespace GestaoRestaurante.Application.Features.Produtos.Queries.GetProdutosBusca;

/// <summary>
/// Handler para busca complexa de produtos
/// </summary>
public sealed class GetProdutosBuscaQueryHandler : IQueryHandler<GetProdutosBuscaQuery, IEnumerable<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public GetProdutosBuscaQueryHandler(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ProdutoDto>>> Handle(GetProdutosBuscaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Usar specification complexa para busca
            var buscaSpec = new ProdutoBuscaCompletaSpecification(
                request.TermoBusca ?? string.Empty,
                request.CategoriaId,
                request.PrecoMinimo,
                request.PrecoMaximo,
                request.ApenasAtivos,
                request.ApenasVenda
            );

            var produtos = await _produtoRepository.GetAllAsync();
            var produtosFiltrados = produtos.Where(buscaSpec.IsSatisfiedBy).ToList();

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtosFiltrados);
            
            return Result<IEnumerable<ProdutoDto>>.Success(produtosDto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProdutoDto>>.Failure($"Erro ao buscar produtos: {ex.Message}");
        }
    }
}