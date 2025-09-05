using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.DeleteSubAgrupamento;

public class DeleteSubAgrupamentoCommandHandler : ICommandHandler<DeleteSubAgrupamentoCommand, bool>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly ICentroCustoRepository _centroCustoRepository;

    public DeleteSubAgrupamentoCommandHandler(
        ISubAgrupamentoRepository subAgrupamentoRepository,
        ICentroCustoRepository centroCustoRepository)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _centroCustoRepository = centroCustoRepository;
    }

    public async Task<Result<bool>> Handle(DeleteSubAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find existing sub-agrupamento
            var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(request.Id);
            if (subAgrupamento == null)
            {
                return Result<bool>.Failure(BusinessRuleMessages.SUBAGRUPAMENTO_NAO_ENCONTRADO);
            }

            // Check if has active centros de custo
            var centrosCusto = await _centroCustoRepository.GetBySubAgrupamentoIdAsync(request.Id);
            if (centrosCusto?.Any() == true)
            {
                var qtdAtivos = centrosCusto.Count(cc => cc.Ativa);
                if (qtdAtivos > 0)
                {
                    return Result<bool>.Failure($"Não é possível desativar o sub-agrupamento. Ele possui {qtdAtivos} centro(s) de custo ativo(s)");
                }
            }

            // Soft delete
            subAgrupamento.Ativa = false;
            await _subAgrupamentoRepository.UpdateAsync(subAgrupamento);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro interno: {ex.Message}");
        }
    }
}