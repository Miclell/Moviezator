using FluentValidation;

namespace Application.Features.Movie.Commands.MarkAsToWatch;

public sealed class MarkAsToWatchCommandValidator : AbstractValidator<MarkAsToWatchCommand>
{
    public MarkAsToWatchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
