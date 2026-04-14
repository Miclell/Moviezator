using Contracts.V1.Responses;
using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Mapster;
using Mediator;
using SharedComponents.Results;

namespace Application.Features.Movie.Queries.GetAll;

public sealed class GetAllQueryHandler(
    IMovieRepository movieRepository) : IQueryHandler<GetAllQuery, CursorPage<MoviesResponse>>
{
    public async ValueTask<CursorPage<MoviesResponse>> Handle(GetAllQuery query, CancellationToken cancellationToken)
    {
        var page = await movieRepository
            .GetAllAsync(query.Adapt<GetMoviesQueryDto>(), cancellationToken);

        var items = page.Items
            .Select(x => new MoviesResponse(
                x.Id,
                x.Title,
                (int)x.Status,
                x.Year,
                x.Genres,
                x.Notes,
                x.Rating?.Value,
                x.WatchedDate))
            .ToArray().AsReadOnly();

        return new CursorPage<MoviesResponse>(items, page.NextCursor, page.HasMore);
    }
}
