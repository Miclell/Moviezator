using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.DTOs.Responses;
using SharedComponents.Results;

namespace Core.Abstractions.Interfaces.Persistence.Repositories;

public interface IMovieRepository
{
    public Task<CursorPage<BaseMovieDto>> GetAllAsync(GetMoviesQueryDto queryDto, CancellationToken ct = default);
}
