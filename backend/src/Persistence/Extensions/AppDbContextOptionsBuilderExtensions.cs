using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Persistence;

public static class AppDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAppDatabase(
        this DbContextOptionsBuilder options,
        NpgsqlDataSource dataSource)
    {
        return options.UseNpgsql(dataSource, ConfigureNpgsql);
    }

    public static DbContextOptionsBuilder<AppDbContext> UseAppDatabase(
        this DbContextOptionsBuilder<AppDbContext> options,
        string connectionString)
    {
        return options.UseNpgsql(connectionString, ConfigureNpgsql);
    }

    public static DbContextOptionsBuilder<AppDbContext> UseAppDatabase(
        this DbContextOptionsBuilder<AppDbContext> options,
        NpgsqlDataSource dataSource)
    {
        return options.UseNpgsql(dataSource, ConfigureNpgsql);
    }

    private static void ConfigureNpgsql(NpgsqlDbContextOptionsBuilder options)
    {
        options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
    }
}
