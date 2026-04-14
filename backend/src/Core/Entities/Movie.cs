using Core.Common;
using Core.Enums;
using Core.Exceptions;
using Core.ValueObjects;
using JetBrains.Annotations;

namespace Core.Entities;

public sealed class Movie : EntityBase<Guid>
{
    [UsedImplicitly]
    private Movie()
    {
    }

    private Movie(
        string title,
        MovieStatus status,
        DateTime? year,
        string[] genres,
        string notes,
        Rating? rating,
        DateTime? watchedDate) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Movie title cannot be empty.");

        Title = title;
        Status = status;
        Year = year;
        Genres = genres;
        Notes = notes;
        Rating = rating;
        WatchedDate = watchedDate;
    }

    public string Title { get; private set; } = null!;
    public MovieStatus Status { get; private set; }
    public DateTime? Year { get; private set; }
    public string[] Genres { get; private set; } = [];
    public string Notes { get; private set; } = string.Empty;
    public Rating? Rating { get; private set; }
    public DateTime? WatchedDate { get; private set; }

    public static Movie CreateToWatch(
        string title,
        DateTime? year,
        string[] genres,
        string notes)
    {
        return new Movie(title, MovieStatus.ToWatch, year, genres, notes, null, null);
    }

    public static Movie CreateWatched(
        string title,
        DateTime? year,
        string[] genres,
        string notes,
        Rating? rating,
        DateTime? watchedDate)
    {
        return new Movie(title, MovieStatus.Watched, year, genres, notes, rating, watchedDate);
    }

    public void UpdateDetails(
        string title,
        DateTime? year,
        string[] genres,
        string notes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Movie title cannot be empty.");

        Title = title;
        Year = year;
        Genres = genres;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsWatched(Rating? rating, DateTime? watchedDate)
    {
        Status = MovieStatus.Watched;
        Rating = rating;
        WatchedDate = watchedDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRating(Rating? rating)
    {
        if (Status != MovieStatus.Watched)
            throw new DomainException("Only watched movies can be rated.");

        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsToWatch()
    {
        Status = MovieStatus.ToWatch;
        Rating = null;
        WatchedDate = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
