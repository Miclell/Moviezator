using Contracts.V1.Movie.Responses;
using Core.Abstractions.DTOs.Requests;
using Mediator;
using SharedComponents.Results;
using SharedComponents.Results.Cursor;
using SharedComponents.Results.Ordering;
using SharedComponents.Results.Ordering.Movie;

namespace Application.Features.Movie.Queries.GetAll;

public sealed record GetAllQuery(
    int Limit,
    string? Cursor = null,
    MovieSortBy SortBy = MovieSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Asc) : IQuery<CursorPage<MoviesResponse>>;
