using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace MigrationService;

public sealed partial class MigrationWorker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<MigrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MigrationServiceStarted(logger);

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationsAsync(dbContext, stoppingToken);

            MigrationServiceCompleted(logger);
        }
        catch (Exception ex)
        {
            Environment.ExitCode = 1;
            MigrationServiceFailed(logger, ex);
            throw;
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    private async Task RunMigrationsAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            ApplyingMigrations(logger);

            await dbContext.Database.MigrateAsync(cancellationToken);

            MigrationCheckCompleted(logger);
        });
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Migration service started.")]
    private static partial void MigrationServiceStarted(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Migration service completed.")]
    private static partial void MigrationServiceCompleted(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Migration service failed.")]
    private static partial void MigrationServiceFailed(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Applying database migrations.")]
    private static partial void ApplyingMigrations(ILogger logger);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Database migration check completed.")]
    private static partial void MigrationCheckCompleted(ILogger logger);
}
