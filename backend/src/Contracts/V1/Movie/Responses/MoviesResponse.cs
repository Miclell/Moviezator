namespace Contracts.V1.Movie.Responses;

public sealed record MoviesResponse(
    Guid Id,
    string Title,
    int Status,
    DateTime? Year,
    string[] Genres,
    string Notes,
    decimal? Rating,
    DateTime? WatchedDate);
