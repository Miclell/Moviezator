namespace Core.Abstractions.DTOs.Requests;

public record GetMoviesQueryDto(
    int Limit,
    string? Cursor = null);
