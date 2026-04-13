using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.DTOs.Responses;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Common.Cursor;
using Persistence.Repositories.Common;
using SharedComponents.Results;

namespace Persistence.Repositories;

public sealed class MovieRepository(DbSet<Movie> set, AppDbContext context)
    : DefaultRepository<Movie, Guid>(set, context), IMovieRepository
{
    private readonly DbSet<Movie> _set = set;

    public async Task<CursorPage<BaseMovieDto>> GetAllAsync(GetMoviesQueryDto queryDto, CancellationToken ct = default)
    {
        var query = _set
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryDto.Cursor))
        {
            var (lastId, date) = EntityCursor.Decode(queryDto.Cursor);

            query = query.Where(m =>
                m.CreatedAt > date ||
                (m.CreatedAt == date && m.Id > lastId));
        }

        var nextPage = await query.Take(queryDto.Limit + 1).ToListAsync(ct);

        var hasMore = nextPage.Count > queryDto.Limit;

        if (hasMore)
            nextPage.RemoveAt(nextPage.Count - 1);

        EntityCursor? nextCursor = null;
        var lastItem = nextPage.LastOrDefault();
        if (lastItem is not null && hasMore)
            nextCursor = EntityCursor.Create(lastItem.Id, lastItem.CreatedAt);

        return new CursorPage<BaseMovieDto>(
            [.. nextPage.Select(m => new BaseMovieDto(
                m.Id,
                m.Title,
                m.Status,
                m.Year,
                m.Genres,
                m.Notes,
                m.Rating,
                m.WatchedDate))],
            nextCursor?.Encode(),
            hasMore);
    }
}
