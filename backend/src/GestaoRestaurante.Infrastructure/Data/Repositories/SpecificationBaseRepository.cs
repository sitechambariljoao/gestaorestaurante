using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório base aprimorado com suporte a Specifications, paginação e ordenação
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class SpecificationBaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly GestaoRestauranteContext Context;
    protected readonly DbSet<T> DbSet;

    protected SpecificationBaseRepository(GestaoRestauranteContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    // Métodos base do IBaseRepository
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.Where(x => x.Ativa).ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.Ativa);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void SoftDelete(T entity)
    {
        entity.Ativa = false;
        DbSet.Update(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(x => x.Id == id && x.Ativa);
    }

    public virtual async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    // Métodos aprimorados com Specifications
    
    /// <summary>
    /// Busca entidades usando uma specification
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(DbSet.AsQueryable(), specification);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Busca uma única entidade usando uma specification
    /// </summary>
    public virtual async Task<T?> FindSingleAsync(ISpecification<T> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(DbSet.AsQueryable(), specification);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Conta entidades usando uma specification
    /// </summary>
    public virtual async Task<int> CountAsync(ISpecification<T> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(DbSet.AsQueryable(), specification);
        return await query.CountAsync();
    }

    /// <summary>
    /// Verifica se existe alguma entidade que satisfaça a specification
    /// </summary>
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(DbSet.AsQueryable(), specification);
        return await query.AnyAsync();
    }

    /// <summary>
    /// Busca com paginação usando specification
    /// </summary>
    public virtual async Task<PagedResult<T>> FindPagedAsync(
        ISpecification<T> specification,
        int pageIndex,
        int pageSize,
        string? orderBy = null,
        bool descending = false)
    {
        var query = SpecificationEvaluator.ApplySpecification(DbSet.AsQueryable(), specification);
        
        // Contar total antes da paginação
        var totalCount = await query.CountAsync();

        // Aplicar ordenação se especificada
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = SpecificationEvaluator.ApplyOrdering(query, orderBy, descending);
        }

        // Aplicar paginação
        query = SpecificationEvaluator.ApplyPaging(query, pageIndex, pageSize);

        var items = await query.ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Busca com múltiplas specifications
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(params ISpecification<T>[] specifications)
    {
        var query = SpecificationEvaluator.ApplySpecifications(DbSet.AsQueryable(), specifications);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Busca com includes dinâmicos
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(
        ISpecification<T> specification,
        params string[] includes)
    {
        var query = DbSet.AsQueryable();

        // Aplicar includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Aplicar specification
        query = SpecificationEvaluator.ApplySpecification(query, specification);

        return await query.ToListAsync();
    }
}

/// <summary>
/// Resultado paginado
/// </summary>
/// <typeparam name="T">Tipo dos itens</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex + 1 < TotalPages;
}