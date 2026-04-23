namespace Persistence.Common.Cursor.DTOs;

public sealed record CursorPageRow<TId, TSortValue, TItem>(
    TId Id,
    DateTime CreatedAt,
    TSortValue SortValue,
    TItem Item)
    where TId : notnull;
