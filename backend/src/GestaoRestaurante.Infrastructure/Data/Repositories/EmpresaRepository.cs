using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class EmpresaRepository(GestaoRestauranteContext context) : SpecificationBaseRepository<Empresa>(context), IEmpresaRepository
{
    public async Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null)
    {
        var query = DbSet.Where(e => e.Cnpj == cnpj && e.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
    {
        var query = DbSet.Where(e => e.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && e.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Empresa>> GetAllWithFiliaisAsync()
    {
        return await DbSet
            .Include(e => e.Filiais.Where(f => f.Ativa))
            .Where(e => e.Ativa)
            .OrderBy(e => e.NomeFantasia)
            .ToListAsync();
    }

    public async Task<Empresa?> GetByIdWithFiliaisAsync(Guid id)
    {
        return await DbSet
            .Include(e => e.Filiais.Where(f => f.Ativa))
            .Include(e => e.AssinaturaAtiva)
            .FirstOrDefaultAsync(e => e.Id == id && e.Ativa);
    }

    // Métodos aprimorados usando Specifications

    /// <summary>
    /// Busca empresas por estado usando specification
    /// </summary>
    public async Task<IEnumerable<Empresa>> FindByEstadoAsync(string estado)
    {
        var specification = new EmpresaPorEstadoSpecification(estado);
        return await FindAsync(specification);
    }

    /// <summary>
    /// Busca empresas ativas com assinatura válida
    /// </summary>
    public async Task<IEnumerable<Empresa>> FindActiveWithValidSubscriptionAsync()
    {
        var activeSpec = new EmpresaAtivaSpecification();
        var subscriptionSpec = new EmpresaComAssinaturaAtivaSpecification();
        return await FindAsync(activeSpec, subscriptionSpec);
    }

    /// <summary>
    /// Busca paginada de empresas com filtros
    /// </summary>
    public async Task<PagedResult<Empresa>> SearchPagedAsync(
        string? nomeFantasia = null,
        string? estado = null,
        string? plano = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var specifications = new List<ISpecification<Empresa>>
        {
            new EmpresaAtivaSpecification()
        };

        if (!string.IsNullOrEmpty(nomeFantasia))
            specifications.Add(new EmpresaPorNomeFantasiaSpecification(nomeFantasia));

        if (!string.IsNullOrEmpty(estado))
            specifications.Add(new EmpresaPorEstadoSpecification(estado));

        if (!string.IsNullOrEmpty(plano))
            specifications.Add(new EmpresaPorPlanoAssinaturaSpecification(plano));

        // Combinar todas as specifications usando AND 
        if (specifications.Count == 0)
            return await FindPagedAsync(new EmpresaAtivaSpecification(), pageIndex, pageSize, "NomeFantasia");
        
        var combinedSpec = specifications.First();
        for (int i = 1; i < specifications.Count; i++)
        {
            combinedSpec = ((Specification<Empresa>)combinedSpec).And(specifications[i]);
        }

        return await FindPagedAsync(combinedSpec, pageIndex, pageSize, "NomeFantasia");
    }
}