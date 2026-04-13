using Core.Exceptions;

namespace Core.ValueObjects;

public readonly record struct Rating
{
    public Rating(decimal value)
    {
        if (value is < 0 or > 10)
            throw new DomainException("Movie rating must be between 0 and 10.");

        if (decimal.Round(value, 1) != value)
            throw new DomainException("Movie rating must have at most one decimal place.");

        Value = value;
    }

    public decimal Value { get; }
}
