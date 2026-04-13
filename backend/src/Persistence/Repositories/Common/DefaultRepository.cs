using Core.Abstractions.Interfaces.Persistence;
using Core.Abstractions.Interfaces.Persistence.Repositories.Common;
using Core.Common;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.Common;

public abstract class DefaultRepository<TEntity, TId>(
    DbSet<TEntity> set,
    AppDbContext context)
    : IDefaultRepository<TEntity, TId> where TEntity : EntityBase<TId>
{
    private DbSet<TEntity> Set => set;
    private AppDbContext Context => context;

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await Set.FindAsync([id], cancellationToken: ct);
    }

    public async Task InsertAsync(TEntity entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, cancellationToken: ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        Set.RemoveRange(entities);
        await Context.SaveChangesAsync(ct);
    }
}
