using Core.Abstractions.Interfaces.Persistence.Repositories;
using Core.Abstractions.Interfaces.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Persistence.Repositories;
using Persistence.Repositories.Common;

namespace Persistence;

public static class ServiceRegistry
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
                                   .GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string is not configured");

        services.AddSingleton(NpgsqlDataSource.Create(connectionString));

        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>()));

        // Repositories
        services.AddScoped<IMovieRepository, MovieRepository>();

        return services;
    }
}
