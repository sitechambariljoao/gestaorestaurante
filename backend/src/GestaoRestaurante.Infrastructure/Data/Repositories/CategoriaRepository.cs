using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class CategoriaRepository : SpecificationBaseRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(GestaoRestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Categoria>> GetByCentroCustoIdAsync(Guid centroCustoId)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.CentroCustoId == centroCustoId && c.Ativa)
            .OrderBy(c => c.Nivel)
            .ThenBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Categoria>> GetByNivelAsync(int nivel)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.Nivel == nivel && c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Categoria>> GetFilhasByPaiIdAsync(Guid categoriaPaiId)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.CategoriaPaiId == categoriaPaiId && c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoInCentroCustoAsync(Guid centroCustoId, string codigo, Guid? excludeId = null)
    {
        var query = DbSet.Where(c => c.CentroCustoId == centroCustoId && c.Codigo == codigo && c.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInCentroCustoAsync(Guid centroCustoId, string nome, Guid? excludeId = null)
    {
        var query = DbSet.Where(c => c.CentroCustoId == centroCustoId && c.Nome == nome && c.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Categoria?> GetByIdWithFilhasAsync(Guid id)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
                .ThenInclude(cc => cc.SubAgrupamento)
                    .ThenInclude(s => s.Agrupamento)
                        .ThenInclude(a => a.Filial)
                            .ThenInclude(f => f.Empresa)
            .Include(c => c.CategoriaPai)
            .Include(c => c.CategoriasFilhas.Where(cf => cf.Ativa))
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativa);
    }

    public async Task<IEnumerable<Categoria>> GetHierarquiaByCentroCustoAsync(Guid centroCustoId)
    {
        return await DbSet
            .Include(c => c.CategoriaPai)
            .Include(c => c.CategoriasFilhas.Where(cf => cf.Ativa))
            .Where(c => c.CentroCustoId == centroCustoId && c.Ativa)
            .OrderBy(c => c.Nivel)
            .ThenBy(c => c.CategoriaPaiId)
            .ThenBy(c => c.Nome)
            .ToListAsync();
    }

    public override async Task<Categoria?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
                .ThenInclude(cc => cc.SubAgrupamento)
                    .ThenInclude(s => s.Agrupamento)
                        .ThenInclude(a => a.Filial)
                            .ThenInclude(f => f.Empresa)
            .Include(c => c.CategoriaPai)
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativa);
    }

    public override async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        return await DbSet
            .Include(c => c.CentroCusto)
                .ThenInclude(cc => cc.SubAgrupamento)
                    .ThenInclude(s => s.Agrupamento)
                        .ThenInclude(a => a.Filial)
                            .ThenInclude(f => f.Empresa)
            .Include(c => c.CategoriaPai)
            .Where(c => c.Ativa)
            .OrderBy(c => c.CentroCusto.SubAgrupamento.Agrupamento.Filial.Empresa.NomeFantasia)
            .ThenBy(c => c.CentroCusto.Nome)
            .ThenBy(c => c.Nivel)
            .ThenBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Categoria>?> GetByCodigoAsync(string codigo)
    {
        if (string.IsNullOrEmpty(codigo))
            return null;

        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.Codigo == codigo && c.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<Categoria>?> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return null;

        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.Nome == nome && c.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<Categoria>> GetByCategoriaPaiIdAsync(Guid categoriaPaiId)
    {
        return await DbSet
            .Include(c => c.CentroCusto)
            .Include(c => c.CategoriaPai)
            .Where(c => c.CategoriaPaiId == categoriaPaiId && c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
}