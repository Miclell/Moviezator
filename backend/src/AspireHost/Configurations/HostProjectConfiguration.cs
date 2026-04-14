using Projects;

namespace AspireHost.Configurations;

internal static class HostProjectConfiguration
{
    internal static IResourceBuilder<ProjectResource> AddHostProjectConfiguration(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresDatabaseResource> database)
    {
        var migrations = builder.AddProject<MigrationService>(DevelopmentDefaults.MigrationServiceProjectName)
            .WithReference(database)
            .WaitFor(database)
            .WithEnvironment(DevelopmentDefaults.AspireModeConfigKey, "true");

        var host = builder.AddProject<Host>(DevelopmentDefaults.HostProjectName)
            .WithReference(database)
            .WaitFor(database)
            .WaitForCompletion(migrations)
            .WithEnvironment(DevelopmentDefaults.AspireModeConfigKey, "true");

        builder.AddJavaScriptApp(
                DevelopmentDefaults.FrontendProjectName,
                "../../../frontend",
                "start")
            .WithNpm(true, "ci")
            .WithHttpEndpoint(targetPort: 5001, port: 5001, isProxied: false)
            .WithEnvironment("API_TARGET", host.GetEndpoint("http"))
            .WaitFor(host);

        return host;
    }
}
