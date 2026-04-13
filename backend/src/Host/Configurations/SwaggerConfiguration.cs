using Microsoft.OpenApi;

namespace Host.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
