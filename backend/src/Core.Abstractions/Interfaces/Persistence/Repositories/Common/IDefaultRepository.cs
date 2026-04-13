namespace Core.Abstractions.Interfaces.Persistence.Repositories.Common;

public interface IDefaultRepository<TEntity, in TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task InsertAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
}
