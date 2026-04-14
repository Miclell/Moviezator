using System.Text.Json;
using Core.Abstractions.Exceptions;

namespace Persistence.Common.Cursor;

internal sealed record EntityCursor(Guid Id, DateTime CreatedAt)
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultBufferSize = 128
    };

    public static EntityCursor Create(Guid id, DateTime createdAt)
    {
        return new EntityCursor(id, createdAt);
    }

    public string Encode()
    {
        return Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(this))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static EntityCursor Decode(string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidCursorException(value, "Cursor is empty");

            var bytes = Convert.FromBase64String(ToBase64(value));
            var utf8Reader = new Utf8JsonReader(bytes);

            var cursor = JsonSerializer.Deserialize<EntityCursor>(ref utf8Reader, _jsonOptions)
                         ?? throw new InvalidCursorException(value, "Deserialized cursor is null");

            if (cursor.Id == Guid.Empty || cursor.CreatedAt == default)
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

    private static string ToBase64(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        var padding = base64.Length % 4;

        return padding switch
        {
            0 => base64,
            2 => base64 + "==",
            3 => base64 + "=",
            _ => throw new FormatException("Invalid Base64Url length")
        };
    }
}
