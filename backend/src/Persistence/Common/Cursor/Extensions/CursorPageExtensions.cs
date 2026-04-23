using Persistence.Common.Cursor.DTOs;
using Persistence.Common.Cursor.Internal.Helpers;
using SharedComponents.Results;

namespace Persistence.Common.Cursor.Extensions;

public static class CursorPageExtensions
{
    public static CursorPage<TItem> ToCursorPage<TId, TItem>(
        this IReadOnlyList<CursorPageRow<TId, DateTime, TItem>> rows,
        int limit)
        where TId : notnull
    {
        return rows.ToCursorPage(limit, CursorSortDefinition.Default());
    }

    public static CursorPage<TItem> ToCursorPage<TId, TSortValue, TItem>(
        this IReadOnlyList<CursorPageRow<TId, TSortValue, TItem>> rows,
        int limit,
        CursorSortDefinition<TSortValue> sortDefinition)
        where TId : notnull
    {
        var pageRows = rows;
        var hasMore = pageRows.Count > limit;

        if (hasMore)
            pageRows = pageRows.Take(limit).ToArray();

        string? nextCursor = null;
        if (hasMore && pageRows.Count > 0)
        {
            var lastItem = pageRows[^1];
            nextCursor = new EntityCursor<TId, TSortValue>(
                lastItem.Id,
                lastItem.CreatedAt,
                lastItem.SortValue,
                sortDefinition.Key,
                sortDefinition.Direction).Encode();
        }

        return new CursorPage<TItem>(
            pageRows.Select(static x => x.Item).ToArray(),
            nextCursor,
            hasMore);
    }
}
