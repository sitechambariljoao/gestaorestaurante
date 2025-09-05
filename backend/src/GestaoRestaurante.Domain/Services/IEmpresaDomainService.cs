using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio complexas relacionadas a empresas
/// </summary>
public interface IEmpresaDomainService
{
    /// <summary>
    /// Valida se uma empresa pode ser criada com os dados fornecidos
    /// </summary>
    Task<Result> ValidateEmpresaCreationAsync(string cnpj, string email);

    /// <summary>
    /// Valida se uma empresa pode ser atualizada com os dados fornecidos
    /// </summary>
    Task<Result> ValidateEmpresaUpdateAsync(Guid empresaId, string cnpj, string email);

    /// <summary>
    /// Valida se uma empresa pode ser inativada
    /// </summary>
    Task<Result> ValidateEmpresaInactivationAsync(Guid empresaId);

    /// <summary>
    /// Calcula se uma empresa pode ter mais filiais baseado no plano
    /// </summary>
    Task<Result<bool>> CanCreateMoreFiliaisAsync(Guid empresaId);

    /// <summary>
    /// Valida hierarquia de empresa-filial-agrupamento
    /// </summary>
    Result ValidateEmpresaHierarchy(Empresa empresa, Filial filial);
}