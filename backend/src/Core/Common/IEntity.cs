namespace Core.Common;

public interface IEntity<out TId>
{
    public TId Id { get; }
}
