using Application.Common.Exceptions;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Enums;
using Core.ValueObjects;
using Mediator;

namespace Application.Features.Movie.Commands.Update;

public sealed class UpdateCommandHandler(IMovieRepository movieRepository) : ICommandHandler<UpdateCommand>
{
    public async ValueTask<Unit> Handle(UpdateCommand command, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Core.Entities.Movie), command.Id);

        if (command.Status is not { } status)
            throw new ArgumentException("Movie status is required.", nameof(command));

        movie.UpdateDetails(command.Title, command.Year, command.Genres ?? [], command.Notes ?? string.Empty);
        switch ((MovieStatus)status)
        {
            case MovieStatus.Watched:
                movie.MarkAsWatched(command.Rating.HasValue ? new Rating(command.Rating.Value) : null,
                    command.WatchedDate);
                break;
            case MovieStatus.ToWatch:
                movie.MarkAsToWatch();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), status, "Unknown movie status");
        }

        await movieRepository.UpdateAsync(movie, cancellationToken);

        return default;
    }
}
