using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.DTOs.Responses;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Common.Cursor.DTOs;
using Persistence.Common.Cursor.Extensions;
using Persistence.Repositories.Common;
using SharedComponents.Results;

namespace Persistence.Repositories;

public sealed class MovieRepository(AppDbContext context)
    : DefaultRepository<Movie, Guid>(context.Movies, context), IMovieRepository
{
    private readonly DbSet<Movie> _set = context.Movies;

    public async Task<CursorPage<BaseMovieDto>> GetAllAsync(GetMoviesQueryDto queryDto, CancellationToken ct = default)
    {
        var rows = await _set
            .AsNoTracking()
            .ApplyCursor<Movie, Guid>(queryDto.Cursor)
            .Select(m => new CursorPageRow<Guid, BaseMovieDto>(
                m.Id,
                m.CreatedAt,
                new BaseMovieDto(
                    m.Id,
                    m.Title,
                    m.Status,
                    m.Year,
                    m.Genres,
                    m.Notes,
                    m.Rating,
                    m.WatchedDate)))
            .Take(queryDto.Limit + 1)
            .ToListAsync(ct);

        return rows.ToCursorPage(queryDto.Limit);
    }
}
