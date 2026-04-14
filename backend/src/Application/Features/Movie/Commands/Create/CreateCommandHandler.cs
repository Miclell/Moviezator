using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Enums;
using Core.ValueObjects;
using Mediator;

namespace Application.Features.Movie.Commands.Create;

public sealed class CreateCommandHandler(IMovieRepository movieRepository) : ICommandHandler<CreateCommand>
{
    public async ValueTask<Unit> Handle(CreateCommand command, CancellationToken cancellationToken)
    {
        if (command.Status is not { } status)
            throw new ArgumentException("Movie status is required.", nameof(command));

        var genres = command.Genres ?? [];
        var notes = command.Notes ?? string.Empty;

        var movie = (MovieStatus)status switch
        {
            MovieStatus.Watched => Core.Entities.Movie.CreateWatched(
                command.Title,
                command.Year,
                genres,
                notes,
                command.Rating.HasValue ? new Rating(command.Rating.Value) : null,
                command.WatchedDate),
            MovieStatus.ToWatch => Core.Entities.Movie.CreateToWatch(
                command.Title,
                command.Year,
                genres,
                notes),
            _ => throw new ArgumentOutOfRangeException(nameof(command), status, "Unknown movie status")
        };

        await movieRepository.InsertAsync(movie, cancellationToken);

        return default;
    }
}
