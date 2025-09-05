using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public class ProdutoRepository : SpecificationBaseRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(GestaoRestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetByCategoriaIdAsync(Guid categoriaId)
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
            .Where(p => p.CategoriaId == categoriaId && p.Ativa)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = DbSet.Where(p => p.Codigo == codigo && p.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNomeInCategoriaAsync(Guid categoriaId, string nome, Guid? excludeId = null)
    {
        var query = DbSet.Where(p => p.CategoriaId == categoriaId && p.Nome == nome && p.Ativa);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Produto>> GetProdutosVendaAsync()
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
            .Where(p => p.ProdutoVenda && p.Ativa)
            .OrderBy(p => p.Categoria.Nome)
            .ThenBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetProdutosEstoqueAsync()
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
            .Where(p => p.ProdutoEstoque && p.Ativa)
            .OrderBy(p => p.Categoria.Nome)
            .ThenBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Produto?> GetByIdWithIngredientesAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
                    .ThenInclude(cc => cc.SubAgrupamento)
                        .ThenInclude(s => s.Agrupamento)
                            .ThenInclude(a => a.Filial)
                                .ThenInclude(f => f.Empresa)
            .Include(p => p.Ingredientes)
                .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativa);
    }

    public override async Task<Produto?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
                    .ThenInclude(cc => cc.SubAgrupamento)
                        .ThenInclude(s => s.Agrupamento)
                            .ThenInclude(a => a.Filial)
                                .ThenInclude(f => f.Empresa)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativa);
    }

    public override async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
                    .ThenInclude(cc => cc.SubAgrupamento)
                        .ThenInclude(s => s.Agrupamento)
                            .ThenInclude(a => a.Filial)
                                .ThenInclude(f => f.Empresa)
            .Where(p => p.Ativa)
            .OrderBy(p => p.Categoria.CentroCusto.SubAgrupamento.Agrupamento.Filial.Empresa.NomeFantasia)
            .ThenBy(p => p.Categoria.Nome)
            .ThenBy(p => p.Nome)
            .ToListAsync();
    }

    // Métodos para CQRS
    public async Task<IEnumerable<Produto>> GetFilteredAsync(Guid? categoriaId, bool? produtoVenda, bool? produtoEstoque)
    {
        var query = DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
                    .ThenInclude(cc => cc.SubAgrupamento)
                        .ThenInclude(s => s.Agrupamento)
                            .ThenInclude(a => a.Filial)
                                .ThenInclude(f => f.Empresa)
            .Where(p => p.Ativa);

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (produtoVenda.HasValue)
        {
            query = query.Where(p => p.ProdutoVenda == produtoVenda.Value);
        }

        if (produtoEstoque.HasValue)
        {
            query = query.Where(p => p.ProdutoEstoque == produtoEstoque.Value);
        }

        return await query
            .OrderBy(p => p.Codigo)
            .ToListAsync();
    }

    public async Task<Produto?> GetByIdWithDetailsAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.Categoria)
                .ThenInclude(c => c.CentroCusto)
                    .ThenInclude(cc => cc.SubAgrupamento)
                        .ThenInclude(s => s.Agrupamento)
                            .ThenInclude(a => a.Filial)
                                .ThenInclude(f => f.Empresa)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativa);
    }

    public async Task<Produto?> GetByIdWithDependenciesAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.Categoria)
            .Include(p => p.ItensPedido)
            .Include(p => p.MovimentacoesEstoque)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativa);
    }

    public async Task<IEnumerable<Produto>> SearchAsync(string termo, bool? produtoVenda, int limite = 50)
    {
        var query = DbSet
            .Include(p => p.Categoria)
            .Where(p => p.Ativa &&
                (p.Codigo.Contains(termo) ||
                 p.Nome.Contains(termo) ||
                 (p.Descricao != null && p.Descricao.Contains(termo))));

        if (produtoVenda.HasValue)
        {
            query = query.Where(p => p.ProdutoVenda == produtoVenda.Value);
        }

        return await query
            .Take(limite)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
}