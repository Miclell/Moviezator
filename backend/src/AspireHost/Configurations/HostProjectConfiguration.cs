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

        return builder.AddProject<Host>(DevelopmentDefaults.HostProjectName)
            .WithReference(database)
            .WaitFor(database)
            .WaitForCompletion(migrations)
            .WithEnvironment(DevelopmentDefaults.AspireModeConfigKey, "true");
    }
}
