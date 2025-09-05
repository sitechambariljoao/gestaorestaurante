using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.DeleteCentroCusto;

public class DeleteCentroCustoCommandHandler : ICommandHandler<DeleteCentroCustoCommand, bool>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public DeleteCentroCustoCommandHandler(
        ICentroCustoRepository centroCustoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _centroCustoRepository = centroCustoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result<bool>> Handle(DeleteCentroCustoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find existing centro de custo
            var centroCusto = await _centroCustoRepository.GetByIdAsync(request.Id);
            if (centroCusto == null)
            {
                return Result<bool>.Failure(BusinessRuleMessages.CENTRO_CUSTO_NAO_ENCONTRADO);
            }

            // Check if has active categorias
            var categorias = await _categoriaRepository.GetByCentroCustoIdAsync(request.Id);
            if (categorias?.Any() == true)
            {
                var qtdAtivas = categorias.Count(c => c.Ativa);
                if (qtdAtivas > 0)
                {
                    return Result<bool>.Failure($"Não é possível desativar o centro de custo. Ele possui {qtdAtivas} categoria(s) ativa(s)");
                }
            }

            // Soft delete
            centroCusto.Ativa = false;
            await _centroCustoRepository.UpdateAsync(centroCusto);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro interno: {ex.Message}");
        }
    }
}