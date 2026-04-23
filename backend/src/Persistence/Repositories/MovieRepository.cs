using Core.Abstractions.DTOs.Requests;
using Core.Abstractions.DTOs.Responses;
using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Persistence.Common.Cursor.DTOs;
using Persistence.Common.Cursor.Extensions;
using Persistence.Repositories.Common;
using SharedComponents.Results;
using SharedComponents.Results.Cursor;
using SharedComponents.Results.Ordering.Movie;

namespace Persistence.Repositories;

public sealed class MovieRepository(AppDbContext context)
    : DefaultRepository<Movie, Guid>(context.Movies, context), IMovieRepository
{
    private readonly DbSet<Movie> _set = context.Movies;

    public async Task<CursorPage<BaseMovieDto>> GetAllAsync(GetMoviesQueryDto queryDto, CancellationToken ct = default)
    {
        return queryDto.SortBy switch
        {
            MovieSortBy.CreatedAt => await GetPageAsync(
                queryDto,
                CursorSortDefinition.Default(queryDto.SortDirection),
                m => m.CreatedAt,
                static m => m.CreatedAt,
                ct),
            MovieSortBy.WatchedDate => await GetPageAsync(
                queryDto,
                new CursorSortDefinition<DateTime?>(nameof(Movie.WatchedDate), queryDto.SortDirection),
                m => m.WatchedDate,
                static m => m.WatchedDate,
                ct),
            MovieSortBy.Rating => await GetPageAsync(
                queryDto,
                new CursorSortDefinition<decimal?>(nameof(Movie.Rating), queryDto.SortDirection),
                m => EF.Property<decimal?>(m, nameof(Movie.Rating)),
                static m => m.Rating?.Value,
                ct),
            _ => throw new ArgumentOutOfRangeException(nameof(queryDto), queryDto.SortBy,
                "Unsupported movie sort field")
        };
    }

    private async Task<CursorPage<BaseMovieDto>> GetPageAsync<TSortValue>(
        GetMoviesQueryDto queryDto,
        CursorSortDefinition<TSortValue> sortDefinition,
        Expression<Func<Movie, TSortValue>> sortValueSelector,
        Func<Movie, TSortValue> getSortValue,
        CancellationToken ct)
    {
        var movies = await _set
            .AsNoTracking()
            .ApplyCursor<Movie, Guid, TSortValue>(queryDto.Cursor, sortDefinition, sortValueSelector)
            .Take(queryDto.Limit + 1)
            .ToListAsync(ct);

        var rows = movies
            .Select(movie => new CursorPageRow<Guid, TSortValue, BaseMovieDto>(
                movie.Id,
                movie.CreatedAt,
                getSortValue(movie),
                new BaseMovieDto(
                    movie.Id,
                    movie.Title,
                    movie.Status,
                    movie.Year,
                    movie.Genres,
                    movie.Notes,
                    movie.Rating,
                    movie.WatchedDate)))
            .ToArray();

        return rows.ToCursorPage(queryDto.Limit, sortDefinition);
    }
}
