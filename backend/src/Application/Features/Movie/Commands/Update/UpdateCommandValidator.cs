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

        RuleFor(x => x.Genres)
            .NotNull()
            .Must(genres => genres.Any(genre => !string.IsNullOrWhiteSpace(genre)))
            .WithMessage("At least one genre is required");

        RuleFor(x => x.Rating)
            .PrecisionScale(10, 1, false)
            .WithMessage("The rating must contain no more than one decimal place");

        RuleFor(x => x.Notes)
            .MaximumLength(4096);
    }
}
