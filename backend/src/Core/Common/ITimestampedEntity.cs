namespace Core.Common;

public interface ITimestampedEntity
{
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
}
