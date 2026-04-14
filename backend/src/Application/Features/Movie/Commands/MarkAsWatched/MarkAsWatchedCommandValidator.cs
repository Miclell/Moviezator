using FluentValidation;

namespace Application.Features.Movie.Commands.MarkAsWatched;

public sealed class MarkAsWatchedCommandValidator : AbstractValidator<MarkAsWatchedCommand>
{
    public MarkAsWatchedCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .PrecisionScale(10, 1, false)
            .WithMessage("The rating must contain no more than one decimal place");
    }
}
