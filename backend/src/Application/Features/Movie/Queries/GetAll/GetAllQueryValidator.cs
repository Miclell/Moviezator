using FluentValidation;
using Core.Abstractions.DTOs.Requests;

namespace Application.Features.Movie.Queries.GetAll;

public sealed class GetAllQueryValidator : AbstractValidator<GetAllQuery>
{
    public GetAllQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Limit must be less than or equal to 100");

        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("SortBy must be a valid movie sort field");

        RuleFor(x => x.SortDirection)
            .IsInEnum().WithMessage("SortDirection must be a valid sort direction");
    }
}
