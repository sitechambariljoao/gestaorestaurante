using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class SubAgrupamentoRepository : SpecificationBaseRepository<SubAgrupamento>, ISubAgrupamentoRepository
{
    public SubAgrupamentoRepository(GestaoRestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SubAgrupamento>> GetByAgrupamentoIdAsync(Guid agrupamentoId)
    {
        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .Where(s => s.AgrupamentoId == agrupamentoId && s.Ativa)
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoInAgrupamentoAsync(Guid agrupamentoId, string codigo, Guid? excludeId = null)
    {
        var query = DbSet.Where(s => s.AgrupamentoId == agrupamentoId && s.Codigo == codigo && s.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInAgrupamentoAsync(Guid agrupamentoId, string nome, Guid? excludeId = null)
    {
        var query = DbSet.Where(s => s.AgrupamentoId == agrupamentoId && s.Nome == nome && s.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<SubAgrupamento?> GetByIdWithCentrosCustoAsync(Guid id)
    {
        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .Include(s => s.CentrosCusto.Where(c => c.Ativa))
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativa);
    }

    public override async Task<SubAgrupamento?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativa);
    }

    public override async Task<IEnumerable<SubAgrupamento>> GetAllAsync()
    {
        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .Where(s => s.Ativa)
            .OrderBy(s => s.Agrupamento.Filial.Empresa.NomeFantasia)
            .ThenBy(s => s.Agrupamento.Nome)
            .ThenBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<SubAgrupamento>?> GetByCodigoAsync(string codigo)
    {
        if (string.IsNullOrEmpty(codigo))
            return null;

        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .Where(s => s.Codigo == codigo && s.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<SubAgrupamento>?> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return null;

        return await DbSet
            .Include(s => s.Agrupamento)
                .ThenInclude(a => a.Filial)
                    .ThenInclude(f => f.Empresa)
            .Where(s => s.Nome == nome && s.Ativa)
            .ToListAsync();
    }
}