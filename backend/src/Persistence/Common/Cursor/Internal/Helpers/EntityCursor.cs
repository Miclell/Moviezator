using System.Text.Json;
using Core.Abstractions.Exceptions;

namespace Persistence.Common.Cursor.Internal.Helpers;

internal sealed record EntityCursor<TId>(TId Id, DateTime CreatedAt)
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public string Encode()
    {
        return ToBase64Url(JsonSerializer.SerializeToUtf8Bytes(this));
    }

    public static EntityCursor<TId> Decode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCursorException(value, "Cursor is empty");

        try
        {
            var bytes = FromBase64Url(value);
            var cursor = JsonSerializer.Deserialize<EntityCursor<TId>>(bytes, _jsonOptions)
                         ?? throw new InvalidCursorException(value, "Deserialized cursor is null");

            if (EqualityComparer<TId>.Default.Equals(cursor.Id, default) || cursor.CreatedAt == default)
                throw new InvalidCursorException(value, "Cursor payload is invalid");

            return cursor;
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

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] FromBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
    }
}
