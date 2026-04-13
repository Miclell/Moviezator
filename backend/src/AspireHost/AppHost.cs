using System.Globalization;
using AspireHost.Configurations;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Sixteen,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}",
        formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AppHost");

    var builder = DistributedApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, config) =>
        config
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var database = builder.AddPostgresConfiguration();

    builder.AddHostProjectConfiguration(database);

    builder.Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AppHost terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
