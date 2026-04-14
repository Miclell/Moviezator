using Mediator;

namespace Application.Features.Movie.Commands.MarkAsToWatch;

public sealed record MarkAsToWatchCommand(Guid Id) : ICommand;
