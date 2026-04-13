namespace AspireHost.Configurations;

internal static class PostgresConfiguration
{
    internal static IResourceBuilder<PostgresDatabaseResource> AddPostgresConfiguration(
        this IDistributedApplicationBuilder builder)
    {
        var postgres = builder.AddPostgres(DevelopmentDefaults.PostgresServerName)
            .WithDataVolume()
            .WithPgAdmin();

        return postgres.AddDatabase(
            DevelopmentDefaults.PostgresConnectionName,
            DevelopmentDefaults.PostgresDatabaseName);
    }
}
