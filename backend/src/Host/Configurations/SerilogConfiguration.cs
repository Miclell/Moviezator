using Serilog;

namespace Host.Configurations;

public static class SerilogConfiguration
{
    public static IHostBuilder AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, services, config) =>
        {
            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();

            var otlpEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                config.WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = otlpEndpoint;
                    options.ResourceAttributes.Add("service.name", context.HostingEnvironment.ApplicationName);
                });
        });

        return host;
    }
}
