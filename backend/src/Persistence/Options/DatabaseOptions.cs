using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Persistence.Options;

public sealed class DatabaseOptions
{
    public const string ConnectionStringName = "DefaultConnection";

    public string ConnectionString { get; set; } = string.Empty;

    public static DatabaseOptions FromConfiguration(IConfiguration configuration)
    {
        return new DatabaseOptions
        {
            ConnectionString = configuration.GetConnectionString(ConnectionStringName) ?? string.Empty
        };
    }
}
