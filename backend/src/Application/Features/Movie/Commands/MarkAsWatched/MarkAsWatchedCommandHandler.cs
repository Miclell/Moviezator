using Application.Common.Exceptions;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.ValueObjects;
using Mediator;

namespace Application.Features.Movie.Commands.MarkAsWatched;

public sealed class MarkAsWatchedCommandHandler(IMovieRepository movieRepository)
    : ICommandHandler<MarkAsWatchedCommand>
{
    public async ValueTask<Unit> Handle(MarkAsWatchedCommand command, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Core.Entities.Movie), command.Id);

        movie.MarkAsWatched(command.Rating.HasValue ? new Rating(command.Rating.Value) : null, command.WatchedDate);

        await movieRepository.UpdateAsync(movie, cancellationToken);

        return default;
    }
}
