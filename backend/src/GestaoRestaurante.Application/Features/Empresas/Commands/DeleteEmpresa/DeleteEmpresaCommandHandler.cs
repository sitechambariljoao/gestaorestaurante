using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Services;

namespace GestaoRestaurante.Application.Features.Empresas.Commands.DeleteEmpresa;

/// <summary>
/// Handler para o comando de exclusão de empresa
/// </summary>
public sealed class DeleteEmpresaCommandHandler : ICommandHandler<DeleteEmpresaCommand>
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IEmpresaDomainService _empresaDomainService;

    public DeleteEmpresaCommandHandler(
        IEmpresaRepository empresaRepository,
        IEmpresaDomainService empresaDomainService)
    {
        _empresaRepository = empresaRepository;
        _empresaDomainService = empresaDomainService;
    }

    public async Task<Result> Handle(DeleteEmpresaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar empresa
            var empresa = await _empresaRepository.GetByIdAsync(request.Id);
            if (empresa == null)
                return Result.Failure("Empresa não encontrada");

            // Validar se pode ser excluída (verificar se não tem dependências ativas)
            if (!empresa.Ativa)
                return Result.Failure("Empresa já está inativa");

            // Inativar empresa (soft delete)
            empresa.Inativar();

            // Persistir
            _empresaRepository.Update(empresa);
            await _empresaRepository.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao excluir empresa: {ex.Message}");
        }
    }
}