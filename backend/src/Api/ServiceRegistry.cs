using Microsoft.Extensions.DependencyInjection;

namespace Api;

public static class ServiceRegistry
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }
}
