namespace Contracts.V1.Movie.Requests;

public sealed record MarkAsWatchedMovieRequest(
    decimal? Rating,
    DateTime? WatchedDate);
