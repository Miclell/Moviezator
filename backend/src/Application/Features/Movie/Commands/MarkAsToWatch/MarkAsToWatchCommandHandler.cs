using Application.Common.Exceptions;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Mediator;

namespace Application.Features.Movie.Commands.MarkAsToWatch;

public sealed class MarkAsToWatchCommandHandler(IMovieRepository movieRepository)
    : ICommandHandler<MarkAsToWatchCommand>
{
    public async ValueTask<Unit> Handle(MarkAsToWatchCommand command, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Core.Entities.Movie), command.Id);

        movie.MarkAsToWatch();

        await movieRepository.UpdateAsync(movie, cancellationToken);

        return default;
    }
}
