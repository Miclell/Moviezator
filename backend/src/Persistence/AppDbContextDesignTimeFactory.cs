using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Persistence.Options;
using Persistence.Options.Helpers;
using Persistence.Options.Validators;

namespace Persistence;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var databaseOptions = new DatabaseOptions
        {
            ConnectionString = AppDbContextDesignTimeConnectionStringResolver.Resolve(args)
        };

        var validationResult = new DatabaseOptionsValidator().Validate(null, databaseOptions);
        if (validationResult.Failed)
            throw new InvalidOperationException(string.Join(Environment.NewLine, validationResult.Failures));

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseAppDatabase(databaseOptions.ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}
