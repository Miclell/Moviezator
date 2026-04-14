using Mediator;

namespace Application.Features.Movie.Commands.Create;

public sealed record CreateCommand(
    string Title,
    int? Status,
    DateTime? Year,
    string[]? Genres,
    string? Notes,
    decimal? Rating,
    DateTime? WatchedDate) : ICommand;
