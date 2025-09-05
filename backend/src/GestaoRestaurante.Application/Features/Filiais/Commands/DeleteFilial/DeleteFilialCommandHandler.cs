using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Features.Filiais.Commands.DeleteFilial;

public class DeleteFilialCommandHandler : ICommandHandler<DeleteFilialCommand, bool>
{
    private readonly IFilialRepository _filialRepository;

    public DeleteFilialCommandHandler(IFilialRepository filialRepository)
    {
        _filialRepository = filialRepository;
    }

    public async Task<Result<bool>> Handle(DeleteFilialCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find existing filial
            var filial = await _filialRepository.GetByIdAsync(request.Id);
            if (filial == null)
            {
                return Result<bool>.Failure(BusinessRuleMessages.FILIAL_NAO_ENCONTRADA);
            }

            // Check if it's the only active filial for the empresa
            var filiaisAtivas = await _filialRepository.GetByEmpresaIdAsync(filial.EmpresaId);
            var filiaisAtivasCount = filiaisAtivas?.Count(f => f.Ativa) ?? 0;

            if (filiaisAtivasCount <= 1)
            {
                return Result<bool>.Failure("Não é possível desativar a filial. A empresa deve ter pelo menos uma filial ativa.");
            }

            // Soft delete
            filial.Ativa = false;
            await _filialRepository.UpdateAsync(filial);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Erro interno: {ex.Message}");
        }
    }
}