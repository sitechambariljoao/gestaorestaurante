using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.Services;

/// <summary>
/// Domain service para regras de negócio complexas relacionadas a empresas
/// </summary>
public class EmpresaDomainService : IEmpresaDomainService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IFilialRepository _filialRepository;

    public EmpresaDomainService(IEmpresaRepository empresaRepository, IFilialRepository filialRepository)
    {
        _empresaRepository = empresaRepository;
        _filialRepository = filialRepository;
    }

    public async Task<Result> ValidateEmpresaCreationAsync(string cnpj, string email)
    {
        var errors = new List<string>();

        // Validar CNPJ único
        if (await _empresaRepository.ExistsByCnpjAsync(cnpj))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Empresa", "CNPJ", cnpj));
        }

        // Validar email único
        if (await _empresaRepository.ExistsByEmailAsync(email))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Empresa", "Email", email));
        }

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    public async Task<Result> ValidateEmpresaUpdateAsync(Guid empresaId, string cnpj, string email)
    {
        var errors = new List<string>();

        // Validar CNPJ único (excluindo a empresa atual)
        if (await _empresaRepository.ExistsByCnpjAsync(cnpj, empresaId))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Empresa", "CNPJ", cnpj));
        }

        // Validar email único (excluindo a empresa atual)
        if (await _empresaRepository.ExistsByEmailAsync(email, empresaId))
        {
            errors.Add(string.Format(BusinessRuleMessages.BusinessRules.DuplicateInContext, 
                "Empresa", "Email", email));
        }

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    public async Task<Result> ValidateEmpresaInactivationAsync(Guid empresaId)
    {
        var empresa = await _empresaRepository.GetByIdWithFiliaisAsync(empresaId);
        
        if (empresa == null)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Empresa"));
        }

        // Verificar se tem filiais ativas
        var filiaisAtivas = empresa.Filiais.Count(f => f.Ativa);
        if (filiaisAtivas > 0)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.CannotInactivateWithDependents, 
                "Empresa", $"{filiaisAtivas} filial(is)"));
        }

        return Result.Success();
    }

    public async Task<Result<bool>> CanCreateMoreFiliaisAsync(Guid empresaId)
    {
        var empresa = await _empresaRepository.GetByIdWithFiliaisAsync(empresaId);
        
        if (empresa == null)
        {
            return Result<bool>.Failure(string.Format(BusinessRuleMessages.BusinessRules.EntityNotFound, "Empresa"));
        }

        if (empresa.AssinaturaAtiva == null)
        {
            return Result<bool>.Failure("Empresa não possui assinatura ativa");
        }

        // Regras de negócio baseadas no plano
        var filiaisAtivas = empresa.Filiais.Count(f => f.Ativa);
        var plano = empresa.AssinaturaAtiva.Plano;

        var canCreate = plano.Nome switch
        {
            "Básico" => filiaisAtivas < 1, // Apenas 1 filial
            "Profissional" => filiaisAtivas < 5, // Até 5 filiais
            "Enterprise" => true, // Ilimitado
            _ => false
        };

        return Result<bool>.Success(canCreate);
    }

    public Result ValidateEmpresaHierarchy(Empresa empresa, Filial filial)
    {
        // Verificar se a filial pertence à empresa
        if (filial.EmpresaId != empresa.Id)
        {
            return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.InvalidHierarchy, 
                "Filial", "Empresa"));
        }

        // Verificar se apenas uma filial pode ser matriz
        if (filial.Matriz)
        {
            var outrasMatrizes = empresa.Filiais.Count(f => f.Matriz && f.Id != filial.Id);
            if (outrasMatrizes > 0)
            {
                return Result.Failure(string.Format(BusinessRuleMessages.BusinessRules.OnlyOneActiveAllowed, 
                    "Filial Matriz", "Empresa"));
            }
        }

        return Result.Success();
    }
}