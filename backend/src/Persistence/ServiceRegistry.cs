using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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

        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        // repo

        return services;
    }
}
