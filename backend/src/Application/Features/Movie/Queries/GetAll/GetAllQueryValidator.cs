using FluentValidation;

namespace Application.Features.Movie.Queries.GetAll;

public sealed class GetAllQueryValidator : AbstractValidator<GetAllQuery>
{
    public GetAllQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Limit must be less than or equal to 100");
    }
}
