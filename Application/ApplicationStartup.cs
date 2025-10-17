using Application.Services;
using Application.Services.TelegramBot;
using Application.Services.TelegramBot.Config;
using Domain.Entities;
using Domain.Entities.TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Application;

public static class ApplicationStartup
{
    public static IServiceCollection TryAddApplicationLayer(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        services.Configure<TelegramBotConfiguration>(configurationManager.GetSection("TelegramBotConfiguration"));
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<TelegramBotConfiguration>>().Value;
            return new TelegramBotClient(config.BotToken);
        });

        services.AddBaseServicesForEntities();
        services.TryAddScoped<BaseService<ApplicationQuestion>>();
        services.TryAddScoped<BaseService<ApplicationQuestionAnswer>>();
        return services;
    }
    
    private static IServiceCollection AddBaseServicesForEntities(this IServiceCollection services)
    {
        var entityAssembly = typeof(Project).Assembly;
        var entityTypes = entityAssembly.GetTypes()
            .Where(t => t.Namespace == "Domain.Entities" && t.IsClass && !t.IsAbstract);

        foreach (var entityType in entityTypes)
        {
            var serviceType = typeof(BaseService<>).MakeGenericType(entityType);
            services.TryAddScoped(serviceType);
        }

        return services;
    }
}