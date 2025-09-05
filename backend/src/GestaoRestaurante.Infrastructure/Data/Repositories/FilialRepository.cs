using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class FilialRepository : SpecificationBaseRepository<Filial>, IFilialRepository
{
    public FilialRepository(GestaoRestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Filial>> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await DbSet
            .Include(f => f.Empresa)
            .Where(f => f.EmpresaId == empresaId && f.Ativa)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null)
    {
        if (string.IsNullOrEmpty(cnpj))
            return false;

        var query = DbSet.Where(f => f.Cnpj == cnpj && f.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        var query = DbSet.Where(f => f.Email == email && f.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null)
    {
        var query = DbSet.Where(f => f.EmpresaId == empresaId && f.Nome == nome && f.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<int> CountActiveByEmpresaIdAsync(Guid empresaId)
    {
        return await DbSet
            .CountAsync(f => f.EmpresaId == empresaId && f.Ativa);
    }

    public async Task<bool> IsUnicaFilialDaEmpresaAsync(Guid filialId)
    {
        var filial = await DbSet.FirstOrDefaultAsync(f => f.Id == filialId);
        if (filial == null)
            return false;

        var totalFiliaisAtivas = await CountActiveByEmpresaIdAsync(filial.EmpresaId);
        return totalFiliaisAtivas <= 1;
    }

    public async Task<Filial?> GetByCnpjAsync(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return null;

        return await DbSet
            .Include(f => f.Empresa)
            .FirstOrDefaultAsync(f => f.Cnpj == cnpj && f.Ativa);
    }

    public async Task<Filial?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        return await DbSet
            .Include(f => f.Empresa)
            .FirstOrDefaultAsync(f => f.Email == email && f.Ativa);
    }

    public override async Task<Filial?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(f => f.Empresa)
            .FirstOrDefaultAsync(f => f.Id == id && f.Ativa);
    }

    public override async Task<IEnumerable<Filial>> GetAllAsync()
    {
        return await DbSet
            .Include(f => f.Empresa)
            .Where(f => f.Ativa)
            .OrderBy(f => f.Empresa.NomeFantasia)
            .ThenBy(f => f.Nome)
            .ToListAsync();
    }

    // Métodos aprimorados usando Specifications e otimizações

    /// <summary>
    /// Busca filiais por empresa usando specification
    /// </summary>
    public async Task<IEnumerable<Filial>> FindByEmpresaAsync(Guid empresaId)
    {
        // Para implementar quando as specifications de Filial forem criadas
        return await GetByEmpresaIdAsync(empresaId);
    }

    /// <summary>
    /// Busca paginada de filiais com filtros
    /// </summary>
    public async Task<PagedResult<Filial>> SearchPagedAsync(
        Guid? empresaId = null,
        string? nome = null,
        string? cidade = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var query = DbSet.Include(f => f.Empresa).Where(f => f.Ativa);

        if (empresaId.HasValue)
            query = query.Where(f => f.EmpresaId == empresaId.Value);

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(f => f.Nome.Contains(nome));

        if (!string.IsNullOrEmpty(cidade))
            query = query.Where(f => f.Endereco.Cidade.Contains(cidade));

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(f => f.Nome)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResult<Filial>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}