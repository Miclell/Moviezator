using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Enums;
using Core.ValueObjects;
using Mediator;

namespace Application.Features.Movie.Commands.Create;

public sealed class CreateCommandHandler(IMovieRepository movieRepository) : ICommandHandler<CreateCommand>
{
    public async ValueTask<Unit> Handle(CreateCommand command, CancellationToken cancellationToken)
    {
        var movie = (MovieStatus)command.Status switch
        {
            MovieStatus.Watched => Core.Entities.Movie.CreateWatched(
                command.Title,
                command.Year,
                command.Genres,
                command.Notes,
                command.Rating.HasValue ? new Rating(command.Rating.Value) : null,
                command.WatchedDate),
            MovieStatus.ToWatch => Core.Entities.Movie.CreateToWatch(
                command.Title,
                command.Year,
                command.Genres,
                command.Notes),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Status, "Unknown movie status")
        };

        await movieRepository.InsertAsync(movie, cancellationToken);

        return default;
    }
}
