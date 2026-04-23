using SharedComponents.Results.Ordering;
using SharedComponents.Results.Ordering.Movie;

namespace Contracts.V1.Movie.Requests;

public record GetMoviesRequest(
    int Limit,
    string? Cursor = null,
    MovieSortBy SortBy = MovieSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Asc);
