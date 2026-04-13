using Api.ExceptionHandling;

namespace Host.Configurations;

public static class ExceptionHandlerConfiguration
{
    public static IServiceCollection AddExceptionHandlerConfig(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    public static IApplicationBuilder UseExceptionHandlerConfig(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }
}
