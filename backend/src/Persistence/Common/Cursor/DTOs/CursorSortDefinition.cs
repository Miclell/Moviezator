using Core.Abstractions.DTOs.Requests;
using Core.Common;

namespace Persistence.Common.Cursor.DTOs;

public sealed record CursorSortDefinition<TSortValue>(
    string Key,
    SortDirection Direction);

public static class CursorSortDefinition
{
    public static CursorSortDefinition<DateTime> Default(SortDirection direction = SortDirection.Asc)
    {
        return new CursorSortDefinition<DateTime>(
            nameof(ITimestampedEntity.CreatedAt),
            direction);
    }
}
