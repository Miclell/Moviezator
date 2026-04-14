namespace Contracts.V1.Movie.Requests;

public record GetMoviesRequest(
    int Limit,
    string? Cursor = null);
