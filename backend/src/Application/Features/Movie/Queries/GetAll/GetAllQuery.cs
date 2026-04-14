using Contracts.V1.Responses;
using Mediator;
using SharedComponents.Results;

namespace Application.Features.Movie.Queries.GetAll;

public sealed record GetAllQuery(
    int Limit,
    string? Cursor = null) : IQuery<CursorPage<MoviesResponse>>;
