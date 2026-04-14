using Core.Entities;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Common;

namespace Persistence.Configurations;

internal sealed class MovieConfiguration : EntityBaseConfiguration<Movie, Guid>
{
    public override void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        base.Configure(builder);

        builder.Property(movie => movie.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(movie => movie.Status);

        builder.Property(movie => movie.Year);

        builder.Property(movie => movie.Genres)
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property(movie => movie.Notes)
            .HasMaxLength(4096)
            .IsRequired();

        builder.Property(movie => movie.Rating)
            .HasConversion(
                rating => rating.HasValue ? rating.Value.Value : (decimal?)null,
                value => value.HasValue ? new Rating(value.Value) : null)
            .HasPrecision(3, 1);

        builder.Property(movie => movie.WatchedDate);
    }
}
