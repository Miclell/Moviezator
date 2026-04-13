using Projects;

namespace AspireHost.Configurations;

internal static class HostProjectConfiguration
{
    internal static IResourceBuilder<ProjectResource> AddHostProjectConfiguration(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresDatabaseResource> database)
    {
        return builder.AddProject<Host>(DevelopmentDefaults.HostProjectName)
            .WithReference(database)
            .WaitFor(database)
            .WithEnvironment(DevelopmentDefaults.AspireModeConfigKey, "true");
    }
}
