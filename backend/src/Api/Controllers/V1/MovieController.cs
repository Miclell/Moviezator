using Application.Features.Movie.Commands.Create;
using Application.Features.Movie.Commands.MarkAsToWatch;
using Application.Features.Movie.Commands.MarkAsWatched;
using Application.Features.Movie.Commands.Remove;
using Application.Features.Movie.Commands.Update;
using Application.Features.Movie.Queries.GetAll;
using Contracts.V1.Common;
using Contracts.V1.Movie.Requests;
using Contracts.V1.Movie.Responses;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharedComponents.Results;
using SharedComponents.Results.Cursor;

namespace Api.Controllers.V1;

[ApiController]
[Route($"{Prefix.V1}/movies")]
public sealed class MovieController(
    IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken ct = default)
    {
        var command = request.Adapt<CreateCommand>();
        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpGet("all")]
    public async Task<ActionResult<CursorPage<MoviesResponse>>> GetAll([FromQuery] GetMoviesRequest request,
        CancellationToken ct = default)
    {
        var query = request.Adapt<GetAllQuery>();
        var response = await mediator.Send(query, ct);

        return response;
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateCommand(
            id,
            request.Title,
            request.Status,
            request.Year,
            request.Genres,
            request.Notes,
            request.Rating,
            request.WatchedDate);

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}/watched")]
    public async Task<IActionResult> MarkAsWatched(Guid id, [FromBody] MarkAsWatchedMovieRequest request,
        CancellationToken ct = default)
    {
        var command = new MarkAsWatchedCommand(id, request.Rating, request.WatchedDate);
        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}/to-watch")]
    public async Task<IActionResult> MarkAsToWatch(Guid id, CancellationToken ct = default)
    {
        var command = new MarkAsToWatchCommand(id);
        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var command = new RemoveCommand(id);
        await mediator.Send(command, ct);

        return NoContent();
    }
}
