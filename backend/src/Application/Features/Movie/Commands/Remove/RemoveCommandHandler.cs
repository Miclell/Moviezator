using Application.Common.Exceptions;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Mediator;

namespace Application.Features.Movie.Commands.Remove;

public sealed class RemoveCommandHandler(IMovieRepository movieRepository) : ICommandHandler<RemoveCommand>
{
    public async ValueTask<Unit> Handle(RemoveCommand command, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Core.Entities.Movie), command.Id);

        await movieRepository.DeleteAsync(movie, cancellationToken);

        return default;
    }
}
