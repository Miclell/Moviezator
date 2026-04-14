using Mediator;

namespace Application.Features.Movie.Commands.Remove;

public sealed record RemoveCommand(Guid Id) : ICommand;
