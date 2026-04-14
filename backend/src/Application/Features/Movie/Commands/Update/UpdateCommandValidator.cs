using Core.Enums;
using FluentValidation;

namespace Application.Features.Movie.Commands.Update;

public sealed class UpdateCommandValidator : AbstractValidator<UpdateCommand>
{
    public UpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Status)
            .Must(BeDefinedMovieStatus)
            .WithMessage("Unknown movie status");

        RuleFor(x => x.Genres)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(genres => genres.Length > 0 && genres.All(genre => !string.IsNullOrWhiteSpace(genre)))
            .WithMessage("Genres must contain at least one non-empty value");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 10)
            .When(x => x.Rating.HasValue)
            .PrecisionScale(10, 1, false)
            .WithMessage("The rating must contain no more than one decimal place");

        When(x => x.Status == (int)MovieStatus.ToWatch, () =>
        {
            RuleFor(x => x.Rating)
                .Null()
                .WithMessage("To-watch movies cannot have a rating");

            RuleFor(x => x.WatchedDate)
                .Null()
                .WithMessage("To-watch movies cannot have a watched date");
        });

        RuleFor(x => x.Notes)
            .MaximumLength(4096);
    }

    private static bool BeDefinedMovieStatus(int status) =>
        Enum.IsDefined((MovieStatus)status);
}
