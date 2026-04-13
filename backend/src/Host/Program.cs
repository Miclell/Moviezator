using System.Globalization;
using Api;
using Application;
using Host.Configurations;
using Persistence;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using ServiceDefaults.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Sixteen,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}",
        formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting host");

    var builder = WebApplication.CreateBuilder(args);

    if (builder.Configuration["USE_ASPIRE"] == "true")
    {
        Log.Information("Aspire mode enabled");
        builder.AddServiceDefaults();
    }
    else
    {
        Log.Information("Aspire mode DISABLED");
    }

    builder.Host.AddSerilog();

    builder.Services.AddExceptionHandlerConfig();
    builder.Services.AddApi();
    builder.Services.AddApplication();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddSwaggerDocs();
    builder.Services.AddHealthChecksConfig();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
        app.UseSwaggerDocs();

    app.UseExceptionHandlerConfig();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecksConfig();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
