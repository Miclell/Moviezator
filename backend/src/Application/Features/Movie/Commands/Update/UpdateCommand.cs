using Mediator;

namespace Application.Features.Movie.Commands.Update;

public sealed record UpdateCommand(
    Guid Id,
    string Title,
    int? Status,
    DateTime? Year,
    string[]? Genres,
    string? Notes,
    decimal? Rating,
    DateTime? WatchedDate) : ICommand;
