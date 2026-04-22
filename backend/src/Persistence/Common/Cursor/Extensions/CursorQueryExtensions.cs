using System.Linq.Expressions;
using Core.Common;
using Persistence.Common.Cursor.DTOs;
using Persistence.Common.Cursor.Internal.Helpers;
using SharedComponents.Results;

namespace Persistence.Common.Cursor.Extensions;

public static class CursorQueryExtensions
{
    public static IQueryable<TEntity> ApplyCursor<TEntity, TId>(
        this IQueryable<TEntity> query,
        string? cursor)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            var decoded = EntityCursor<TId>.Decode(cursor);
            query = query.Where(BuildCursorPredicate<TEntity, TId>(decoded.CreatedAt, decoded.Id));
        }

        return query
            .OrderBy(e => e.CreatedAt)
            .ThenBy(e => e.Id);
    }

    public static CursorPage<TItem> ToCursorPage<TId, TItem>(
        this IReadOnlyList<CursorPageRow<TId, TItem>> rows,
        int limit)
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
            nextCursor = new EntityCursor<TId>(lastItem.Id, lastItem.CreatedAt).Encode();
        }

        return new CursorPage<TItem>(
            pageRows.Select(static x => x.Item).ToArray(),
            nextCursor,
            hasMore);
    }

    private static Expression<Func<TEntity, bool>> BuildCursorPredicate<TEntity, TId>(
        DateTime cursorDate,
        TId cursorId)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        var param = Expression.Parameter(typeof(TEntity), "e");

        var dateProp = Expression.Property(param, nameof(EntityBase<>.CreatedAt));
        var idProp = Expression.Property(param, nameof(EntityBase<>.Id));

        var dateGreater = Expression.GreaterThan(dateProp, Expression.Constant(cursorDate));
        var dateEqual = Expression.Equal(dateProp, Expression.Constant(cursorDate));
        var idGreater = Expression.GreaterThan(idProp, Expression.Constant(cursorId));

        var body = Expression.OrElse(
            dateGreater,
            Expression.AndAlso(dateEqual, idGreater)
        );

        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }
}
