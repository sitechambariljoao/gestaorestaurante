using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class CentroCustoRepository : SpecificationBaseRepository<CentroCusto>, ICentroCustoRepository
{
    public CentroCustoRepository(GestaoRestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CentroCusto>> GetBySubAgrupamentoIdAsync(Guid subAgrupamentoId)
    {
        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Where(c => c.SubAgrupamentoId == subAgrupamentoId && c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoInSubAgrupamentoAsync(Guid subAgrupamentoId, string codigo, Guid? excludeId = null)
    {
        var query = DbSet.Where(c => c.SubAgrupamentoId == subAgrupamentoId && c.Codigo == codigo && c.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInSubAgrupamentoAsync(Guid subAgrupamentoId, string nome, Guid? excludeId = null)
    {
        var query = DbSet.Where(c => c.SubAgrupamentoId == subAgrupamentoId && c.Nome == nome && c.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<CentroCusto?> GetByIdWithCategoriasAsync(Guid id)
    {
        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Include(c => c.Categorias.Where(cat => cat.Ativa))
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativa);
    }

    public async Task<IEnumerable<CentroCusto>> GetByFilialIdAsync(Guid filialId)
    {
        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Where(c => c.SubAgrupamento.Agrupamento.FilialId == filialId && c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public override async Task<CentroCusto?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativa);
    }

    public override async Task<IEnumerable<CentroCusto>> GetAllAsync()
    {
        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Where(c => c.Ativa)
            .OrderBy(c => c.SubAgrupamento.Agrupamento.Filial.Empresa.NomeFantasia)
            .ThenBy(c => c.SubAgrupamento.Agrupamento.Nome)
            .ThenBy(c => c.SubAgrupamento.Nome)
            .ThenBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<CentroCusto>?> GetByCodigoAsync(string codigo)
    {
        if (string.IsNullOrEmpty(codigo))
            return null;

        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Where(c => c.Codigo == codigo && c.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<CentroCusto>?> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return null;

        return await DbSet
            .Include(c => c.SubAgrupamento)
                .ThenInclude(s => s.Agrupamento)
                    .ThenInclude(a => a.Filial)
                        .ThenInclude(f => f.Empresa)
            .Where(c => c.Nome == nome && c.Ativa)
            .ToListAsync();
    }
}