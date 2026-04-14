using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.DTOs.Responses;
using Core.Abstractions.Interfaces.Persistence.Repositories.Common;
using Core.Entities;
using SharedComponents.Results;

namespace Core.Abstractions.Interfaces.Persistence.Repositories;

public interface IMovieRepository : IDefaultRepository<Movie, Guid>
{
    public Task<CursorPage<BaseMovieDto>> GetAllAsync(GetMoviesQueryDto queryDto, CancellationToken ct = default);
}
