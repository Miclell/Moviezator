using FluentValidation;

namespace Application.Features.Movie.Commands.Create;

public sealed class CreateCommandValidator : AbstractValidator<CreateCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Rating)
            .PrecisionScale(10, 1, false)
            .WithMessage("The rating must contain no more than one decimal place");

        RuleFor(x => x.Notes)
            .MaximumLength(4096);
    }
}
