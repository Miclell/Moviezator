using System.Text.Json;
using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.Exceptions;
using Persistence.Common.Cursor.DTOs;
using SharedComponents.Results.Ordering;

namespace Persistence.Common.Cursor.Internal.Helpers;

internal sealed record EntityCursor<TId, TSortValue>(
    TId Id,
    DateTime CreatedAt,
    TSortValue SortValue,
    string SortKey,
    SortDirection Direction)
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public string Encode()
    {
        return ToBase64Url(JsonSerializer.SerializeToUtf8Bytes(this));
    }

    public static EntityCursor<TId, TSortValue> Decode(string value, CursorSortDefinition<TSortValue> sortDefinition)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCursorException(value, "Cursor is empty");

        try
        {
            var bytes = FromBase64Url(value);
            var cursor = JsonSerializer.Deserialize<EntityCursor<TId, TSortValue>>(bytes, _jsonOptions);

            if (cursor is not null)
            {
                if (ShouldUseLegacyFallback(cursor, sortDefinition)
                    && TryDecodeLegacy(value, bytes, sortDefinition, out var legacyDecodedCursor))
                    return legacyDecodedCursor;

                Validate(cursor, value, sortDefinition);
                return cursor;
            }

            if (TryDecodeLegacy(value, bytes, sortDefinition, out var legacyCursor))
                return legacyCursor;

            throw new InvalidCursorException(value, "Deserialized cursor is null");
        }
        catch (FormatException)
        {
            throw new InvalidCursorException(value, "Invalid Base64 format");
        }
        catch (JsonException)
        {
            throw new InvalidCursorException(value, "Invalid JSON format");
        }
    }

    private static bool ShouldUseLegacyFallback(
        EntityCursor<TId, TSortValue> cursor,
        CursorSortDefinition<TSortValue> sortDefinition)
    {
        return IsLegacyCreatedAtCursor(sortDefinition)
               && string.IsNullOrWhiteSpace(cursor.SortKey);
    }

    private static void Validate(
        EntityCursor<TId, TSortValue> cursor,
        string value,
        CursorSortDefinition<TSortValue> sortDefinition)
    {
        if (EqualityComparer<TId>.Default.Equals(cursor.Id, default) || cursor.CreatedAt == default)
            throw new InvalidCursorException(value, "Cursor payload is invalid");

        if (!string.Equals(cursor.SortKey, sortDefinition.Key, StringComparison.Ordinal))
            throw new InvalidCursorException(value, "Cursor sort key does not match request sort key");

        if (cursor.Direction != sortDefinition.Direction)
            throw new InvalidCursorException(value, "Cursor sort direction does not match request sort direction");
    }

    private static bool TryDecodeLegacy(
        string value,
        byte[] bytes,
        CursorSortDefinition<TSortValue> sortDefinition,
        out EntityCursor<TId, TSortValue> cursor)
    {
        cursor = default!;

        if (!IsLegacyCreatedAtCursor(sortDefinition))
            return false;

        var legacyCursor = JsonSerializer.Deserialize<LegacyEntityCursor<TId>>(bytes, _jsonOptions);
        if (legacyCursor is null)
            return false;

        if (EqualityComparer<TId>.Default.Equals(legacyCursor.Id, default) || legacyCursor.CreatedAt == default)
            throw new InvalidCursorException(value, "Cursor payload is invalid");

        cursor = new EntityCursor<TId, TSortValue>(
            legacyCursor.Id,
            legacyCursor.CreatedAt,
            (TSortValue)(object)legacyCursor.CreatedAt,
            sortDefinition.Key,
            sortDefinition.Direction);

        return true;
    }

    private static bool IsLegacyCreatedAtCursor(CursorSortDefinition<TSortValue> sortDefinition)
    {
        return typeof(TSortValue) == typeof(DateTime)
               && string.Equals(sortDefinition.Key, nameof(Core.Common.ITimestampedEntity.CreatedAt),
                   StringComparison.Ordinal)
               && sortDefinition.Direction == SortDirection.Asc;
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] FromBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
    }

    private sealed record LegacyEntityCursor<TCursorId>(TCursorId Id, DateTime CreatedAt);
}
