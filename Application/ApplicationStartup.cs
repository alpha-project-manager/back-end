using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationStartup
{
    public static IServiceCollection TryAddApplicationLayer(this IServiceCollection services)
    {
        // services.TryAddScoped<BaseService<Project>>();
        return services;
    }
}