using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.DeleteCategoria;

public class DeleteCategoriaCommandHandler : ICommandHandler<DeleteCategoriaCommand, bool>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IProdutoRepository _produtoRepository;

    public DeleteCategoriaCommandHandler(
        ICategoriaRepository categoriaRepository,
        IProdutoRepository produtoRepository)
    {
        _categoriaRepository = categoriaRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<Result<bool>> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find existing category
            var categoria = await _categoriaRepository.GetByIdAsync(request.Id);
            if (categoria == null)
            {
                return Result<bool>.Failure(BusinessRuleMessages.CATEGORIA_NAO_ENCONTRADA);
            }

            // Check if has sub-categories
            var subcategorias = await _categoriaRepository.GetByCategoriaPaiIdAsync(request.Id);
            if (subcategorias?.Any() == true)
            {
                var qtdAtivas = subcategorias.Count(s => s.Ativa);
                if (qtdAtivas > 0)
                {
                    return Result<bool>.Failure($"Não é possível desativar a categoria. Ela possui {qtdAtivas} categoria(s) filha(s) ativa(s)");
                }
            }

            // Check if has products
            var produtos = await _produtoRepository.GetByCategoriaIdAsync(request.Id);
            if (produtos?.Any() == true)
            {
                var qtdAtivos = produtos.Count(p => p.Ativa);
                if (qtdAtivos > 0)
                {
                    return Result<bool>.Failure($"Não é possível desativar a categoria. Ela possui {qtdAtivos} produto(s) ativo(s)");
                }
            }

            // Soft delete
            categoria.Ativa = false;
            await _categoriaRepository.UpdateAsync(categoria);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro interno: {ex.Message}");
        }
    }
}