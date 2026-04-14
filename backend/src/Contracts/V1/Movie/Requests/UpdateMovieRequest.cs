namespace Contracts.V1.Movie.Requests;

public sealed record UpdateMovieRequest(
    string Title,
    int Status,
    DateTime? Year,
    string[] Genres,
    string Notes,
    decimal? Rating,
    DateTime? WatchedDate);
