using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Abstractions.Interfaces.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Persistence.Options;
using Persistence.Options.Validators;
using Persistence.Repositories;
using Persistence.Repositories.Common;

namespace Persistence;

public static class ServiceRegistry
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAppDbContext(configuration);

        services.AddScoped<IMovieRepository, MovieRepository>();

        return services;
    }

    public static IServiceCollection AddAppDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Configure(options =>
            {
                var databaseOptions = DatabaseOptions.FromConfiguration(configuration);
                options.ConnectionString = databaseOptions.ConnectionString;
            })
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            return NpgsqlDataSource.Create(options.ConnectionString);
        });

        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseAppDatabase(sp.GetRequiredService<NpgsqlDataSource>()));

        return services;
    }
}
