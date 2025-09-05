using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> 
    where TEntity : BaseEntity
{
    protected readonly GestaoRestauranteContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(GestaoRestauranteContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet
            .Where(e => e.Ativa)
            .OrderBy(e => e.DataCriacao)
            .ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id && e.Ativa);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.DataCriacao = DateTime.UtcNow;
        entity.Ativa = true;
        
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void SoftDelete(TEntity entity)
    {
        entity.Ativa = false;
        _dbSet.Update(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && e.Ativa);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}