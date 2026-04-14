namespace Contracts.V1.Requests;

public record GetMoviesRequest(
    int Limit,
    string? Cursor = null);
