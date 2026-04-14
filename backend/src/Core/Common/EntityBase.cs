namespace Core.Common;

public abstract class EntityBase<TId> : IEntity<TId>, ITimestampedEntity
{
    protected EntityBase()
    {
    }

    protected EntityBase(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
}
