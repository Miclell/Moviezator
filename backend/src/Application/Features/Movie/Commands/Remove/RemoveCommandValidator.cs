using FluentValidation;

namespace Application.Features.Movie.Commands.Remove;

public sealed class RemoveCommandValidator : AbstractValidator<RemoveCommand>
{
    public RemoveCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
