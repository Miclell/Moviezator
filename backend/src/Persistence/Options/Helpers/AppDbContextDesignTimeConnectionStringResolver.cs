using System.Text.Json;

namespace Persistence.Options.Helpers;

internal static class AppDbContextDesignTimeConnectionStringResolver
{
    public static string Resolve(string[] args)
    {
        var connectionString = GetConnectionStringFromArgs(args)
                               ?? Environment.GetEnvironmentVariable($"ConnectionStrings__{DatabaseOptions.ConnectionStringName}")
                               ?? Environment.GetEnvironmentVariable($"ConnectionStrings:{DatabaseOptions.ConnectionStringName}")
                               ?? Environment.GetEnvironmentVariable(DatabaseOptions.ConnectionStringName)
                               ?? GetConnectionStringFromHostSettings();

        return string.IsNullOrWhiteSpace(connectionString)
            ? string.Empty
            : connectionString;
    }

    private static string? GetConnectionStringFromArgs(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            const string connectionPrefix = "--connection=";
            const string connectionStringPrefix = "--connection-string=";

            if (args[i].StartsWith(connectionPrefix, StringComparison.OrdinalIgnoreCase))
                return args[i][connectionPrefix.Length..];

            if (args[i].StartsWith(connectionStringPrefix, StringComparison.OrdinalIgnoreCase))
                return args[i][connectionStringPrefix.Length..];

            if (!IsConnectionStringArgument(args[i]))
                continue;

            if (i + 1 >= args.Length)
                throw new ArgumentException($"Missing value for '{args[i]}' argument.", nameof(args));

            return args[i + 1];
        }

        return null;
    }

    private static bool IsConnectionStringArgument(string arg)
    {
        return string.Equals(arg, "--connection", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(arg, "--connection-string", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetConnectionStringFromHostSettings()
    {
        var hostDirectory = FindHostDirectory();
        if (hostDirectory is null)
            return null;

        return GetConnectionStringFromJson(Path.Combine(hostDirectory, "appsettings.Development.json")) ??
               GetConnectionStringFromJson(Path.Combine(hostDirectory, "appsettings.json"));
    }

    private static string? FindHostDirectory()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current is not null)
        {
            var hostDirectory = Path.Combine(current.FullName, "backend", "src", "Host");
            if (Directory.Exists(hostDirectory))
                return hostDirectory;

            current = current.Parent;
        }

        return null;
    }

    private static string? GetConnectionStringFromJson(string path)
    {
        if (!File.Exists(path))
            return null;

        using var document = JsonDocument.Parse(File.ReadAllText(path));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) ||
            !connectionStrings.TryGetProperty(DatabaseOptions.ConnectionStringName, out var connectionString) ||
            connectionString.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return connectionString.GetString();
    }
}
