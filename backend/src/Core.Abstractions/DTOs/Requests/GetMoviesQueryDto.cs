using SharedComponents.Results.Ordering;
using SharedComponents.Results.Ordering.Movie;

namespace Core.Abstractions.DTOs.Requests;

public record GetMoviesQueryDto(
    int Limit,
    string? Cursor = null,
    MovieSortBy SortBy = MovieSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Asc);
