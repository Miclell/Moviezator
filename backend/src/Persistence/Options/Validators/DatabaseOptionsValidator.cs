using Microsoft.Extensions.Options;

namespace Persistence.Options.Validators;

public sealed class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        return string.IsNullOrWhiteSpace(options.ConnectionString)
            ? ValidateOptionsResult.Fail(
                $"Connection string '{DatabaseOptions.ConnectionStringName}' is not configured.")
            : ValidateOptionsResult.Success;
    }
}
