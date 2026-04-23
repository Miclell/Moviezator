using System.Linq.Expressions;
using Core.Abstractions.DTOs.Requests;
using Core.Common;
using Persistence.Common.Cursor.DTOs;
using Persistence.Common.Cursor.Internal.Helpers;

namespace Persistence.Common.Cursor.Extensions;

public static class CursorQueryExtensions
{
    public static IQueryable<TEntity> ApplyCursor<TEntity, TId>(
        this IQueryable<TEntity> query,
        string? cursor)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        return query.ApplyCursor<TEntity, TId, DateTime>(
            cursor,
            CursorSortDefinition.Default(),
            static entity => entity.CreatedAt);
    }

    public static IQueryable<TEntity> ApplyCursor<TEntity, TId, TSortValue>(
        this IQueryable<TEntity> query,
        string? cursor,
        CursorSortDefinition<TSortValue> sortDefinition,
        Expression<Func<TEntity, TSortValue>> sortValueSelector)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            var decoded = EntityCursor<TId, TSortValue>.Decode(cursor, sortDefinition);
            query = query.Where(BuildCursorPredicate(decoded, sortDefinition, sortValueSelector));
        }

        return ApplyOrdering<TEntity, TId, TSortValue>(query, sortDefinition, sortValueSelector);
    }

    private static IQueryable<TEntity> ApplyOrdering<TEntity, TId, TSortValue>(
        IQueryable<TEntity> query,
        CursorSortDefinition<TSortValue> sortDefinition,
        Expression<Func<TEntity, TSortValue>> sortValueSelector)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        var direction = NormalizeDirection(sortDefinition.Direction);

        if (!CanBeNull(typeof(TSortValue)))
        {
            return direction == SortDirection.Asc
                ? query.OrderBy(sortValueSelector).ThenBy(e => e.CreatedAt).ThenBy(e => e.Id)
                : query.OrderByDescending(sortValueSelector).ThenByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id);
        }

        var isNullSelector = BuildIsNullSelector(sortValueSelector);

        return direction == SortDirection.Asc
            ? query.OrderBy(isNullSelector).ThenBy(sortValueSelector).ThenBy(e => e.CreatedAt).ThenBy(e => e.Id)
            : query.OrderBy(isNullSelector).ThenByDescending(sortValueSelector).ThenByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id);
    }

    private static Expression<Func<TEntity, bool>> BuildCursorPredicate<TEntity, TId, TSortValue>(
        EntityCursor<TId, TSortValue> cursor,
        CursorSortDefinition<TSortValue> sortDefinition,
        Expression<Func<TEntity, TSortValue>> sortValueSelector)
        where TEntity : EntityBase<TId>
        where TId : notnull
    {
        var param = Expression.Parameter(typeof(TEntity), "entity");

        var idBody = Expression.Property(param, nameof(EntityBase<TId>.Id));
        var createdAtBody = Expression.Property(param, nameof(EntityBase<TId>.CreatedAt));
        var sortValueBody = ReplaceParameter(sortValueSelector, param);

        var createdAtComparison = BuildComparison(createdAtBody, Expression.Constant(cursor.CreatedAt), sortDefinition.Direction);
        var createdAtEqual = Expression.Equal(createdAtBody, Expression.Constant(cursor.CreatedAt));
        var idComparison = BuildComparison(idBody, Expression.Constant(cursor.Id, typeof(TId)), sortDefinition.Direction);

        Expression tieBreaker = Expression.OrElse(
            createdAtComparison,
            Expression.AndAlso(createdAtEqual, idComparison));

        if (!CanBeNull(typeof(TSortValue)))
        {
            var sortValueConstant = Expression.Constant(cursor.SortValue, typeof(TSortValue));
            var sortValueComparison = BuildComparison(sortValueBody, sortValueConstant, sortDefinition.Direction);
            var sortValueEqual = Expression.Equal(sortValueBody, sortValueConstant);

            var body = Expression.OrElse(
                sortValueComparison,
                Expression.AndAlso(sortValueEqual, tieBreaker));

            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }

        var nullConstant = Expression.Constant(null, typeof(TSortValue));
        var entitySortValueIsNull = Expression.Equal(sortValueBody, nullConstant);
        var entitySortValueIsNotNull = Expression.Not(entitySortValueIsNull);

        if (IsNull(cursor.SortValue))
        {
            var body = Expression.AndAlso(entitySortValueIsNull, tieBreaker);
            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }

        var nullableSortValueConstant = Expression.Constant(cursor.SortValue, typeof(TSortValue));
        var nullableSortValueComparison = BuildComparison(sortValueBody, nullableSortValueConstant, sortDefinition.Direction);
        var nullableSortValueEqual = Expression.Equal(sortValueBody, nullableSortValueConstant);

        var bodyWithNullsLast = Expression.OrElse(
            entitySortValueIsNull,
            Expression.AndAlso(
                entitySortValueIsNotNull,
                Expression.OrElse(
                    nullableSortValueComparison,
                    Expression.AndAlso(nullableSortValueEqual, tieBreaker))));

        return Expression.Lambda<Func<TEntity, bool>>(bodyWithNullsLast, param);
    }

    private static Expression<Func<TEntity, bool>> BuildIsNullSelector<TEntity, TSortValue>(
        Expression<Func<TEntity, TSortValue>> sortValueSelector)
    {
        var param = Expression.Parameter(typeof(TEntity), "entity");
        var sortValueBody = ReplaceParameter(sortValueSelector, param);
        var body = Expression.Equal(sortValueBody, Expression.Constant(null, typeof(TSortValue)));

        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    private static Expression ReplaceParameter<TEntity, TValue>(
        Expression<Func<TEntity, TValue>> selector,
        ParameterExpression target)
    {
        return new ReplaceParameterVisitor(selector.Parameters[0], target).Visit(selector.Body)
               ?? throw new InvalidOperationException("Failed to replace selector parameter");
    }

    private static BinaryExpression BuildComparison(Expression left, Expression right, SortDirection direction)
    {
        return NormalizeDirection(direction) switch
        {
            SortDirection.Asc => Expression.GreaterThan(left, right),
            SortDirection.Desc => Expression.LessThan(left, right),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported sort direction")
        };
    }

    private static bool CanBeNull(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
    }

    private static bool IsNull<T>(T value)
    {
        return value is null;
    }

    private static SortDirection NormalizeDirection(SortDirection direction)
    {
        return direction switch
        {
            SortDirection.Asc => SortDirection.Asc,
            SortDirection.Desc => SortDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported sort direction")
        };
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? target : base.VisitParameter(node);
        }
    }
}
