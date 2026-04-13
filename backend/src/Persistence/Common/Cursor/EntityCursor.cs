using System.Text;
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
        var json = JsonSerializer.Serialize(this);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static EntityCursor Decode(string value)
    {
        try
        {
            var bytes = Convert.FromBase64String(value);
            var utf8Reader = new Utf8JsonReader(bytes);

            return JsonSerializer.Deserialize<EntityCursor>(ref utf8Reader, _jsonOptions)
                   ?? throw new InvalidCursorException(value, "Deserialized cursor is null");
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
}
