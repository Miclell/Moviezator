namespace SharedComponents.Results.Cursor;

public sealed record CursorPage<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore);
