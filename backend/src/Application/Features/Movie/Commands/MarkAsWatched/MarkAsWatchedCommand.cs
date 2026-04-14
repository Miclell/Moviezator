using Mediator;

namespace Application.Features.Movie.Commands.MarkAsWatched;

public sealed record MarkAsWatchedCommand(
    Guid Id,
    decimal? Rating,
    DateTime? WatchedDate) : ICommand;
