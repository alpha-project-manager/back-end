using Microsoft.Extensions.DependencyInjection;
using TeamProjectConnection.Intefaces;

namespace TeamProjectConnection;

public static class TeamProjectConnectionStartup
{
    public static IServiceCollection AddTeamProjectConnection(this IServiceCollection services)
    {
        services.AddScoped<ITeamProAuthManager, KeyCloakTeamProAuth>();
        services.AddScoped<ITeamProjectManager, TeamProjectManager>();
        return services;
    }
}