namespace Persistence.Common.Cursor.DTOs;

public sealed record CursorPageRow<TId, TItem>(
    TId Id,
    DateTime CreatedAt,
    TItem Item)
    where TId : notnull;
