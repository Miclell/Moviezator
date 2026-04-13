using Core.Enums;
using Core.ValueObjects;

namespace Core.Abstractions.DTOs.Responses;

public sealed record BaseMovieDto(
    Guid Id,
    string Title,
    MovieStatus Status,
    DateTime? Year,
    string[] Genres,
    string Notes,
    Rating? Rating,
    DateTime? WatchedDate);
