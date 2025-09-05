using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class AgrupamentoRepository(GestaoRestauranteContext context) : SpecificationBaseRepository<Agrupamento>(context), IAgrupamentoRepository
{
    public async Task<bool> ExistsByCodigoInEmpresaAsync(Guid empresaId, string codigo, Guid? excludeId = null)
    {
        var query = DbSet
            .Include(a => a.Filial)
            .Where(a => a.Filial.EmpresaId == empresaId && a.Codigo == codigo && a.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInEmpresaAsync(Guid empresaId, string nome, Guid? excludeId = null)
    {
        var query = DbSet
            .Include(a => a.Filial)
            .Where(a => a.Filial.EmpresaId == empresaId && a.Nome == nome && a.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Agrupamento>> GetByFilialIdAsync(Guid filialId)
    {
        return await DbSet
            .Include(a => a.Filial)
            .Where(a => a.FilialId == filialId && a.Ativa)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Agrupamento?> GetByIdWithSubAgrupamentosAsync(Guid id)
    {
        return await DbSet
            .Include(a => a.Filial)
            .Include(a => a.SubAgrupamentos.Where(s => s.Ativa))
            .FirstOrDefaultAsync(a => a.Id == id && a.Ativa);
    }

    public async Task<Agrupamento?> GetByIdWithFiliaisAsync(Guid id)
    {
        return await DbSet
            .Include(a => a.Filial)
            .FirstOrDefaultAsync(a => a.Id == id && a.Ativa);
    }

    public override async Task<Agrupamento?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(a => a.Filial)
            .FirstOrDefaultAsync(a => a.Id == id && a.Ativa);
    }

    public override async Task<IEnumerable<Agrupamento>> GetAllAsync()
    {
        return await DbSet
            .Include(a => a.Filial)
            .ThenInclude(f => f.Empresa)
            .Where(a => a.Ativa)
            .OrderBy(a => a.Filial.Empresa.NomeFantasia)
            .ThenBy(a => a.Filial.Nome)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    // Métodos aprimorados usando Specifications e otimizações

    /// <summary>
    /// Busca paginada de agrupamentos com filtros
    /// </summary>
    public async Task<PagedResult<Agrupamento>> SearchPagedAsync(
        Guid? empresaId = null,
        Guid? filialId = null,
        string? nome = null,
        string? codigo = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var query = DbSet
            .Include(a => a.Filial)
            .ThenInclude(f => f.Empresa)
            .Where(a => a.Ativa);

        if (empresaId.HasValue)
            query = query.Where(a => a.Filial.EmpresaId == empresaId.Value);

        if (filialId.HasValue)
            query = query.Where(a => a.FilialId == filialId.Value);

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(a => a.Nome.Contains(nome));

        if (!string.IsNullOrEmpty(codigo))
            query = query.Where(a => a.Codigo.Contains(codigo));

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(a => a.Nome)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResult<Agrupamento>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // Método para CQRS
    public async Task<Agrupamento?> GetByIdWithDependenciesAsync(Guid id)
    {
        return await DbSet
            .Include(a => a.Filial)
            .Include(a => a.SubAgrupamentos) // Incluir sub-agrupamentos para verificar dependências
            .FirstOrDefaultAsync(a => a.Id == id && a.Ativa);
    }
}