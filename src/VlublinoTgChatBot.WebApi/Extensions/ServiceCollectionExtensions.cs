using VlublinoTgChatBot.WebApi.Options;
using VlublinoTgChatBot.WebApi.Services;

namespace VlublinoTgChatBot.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TelegramReminderOptions>()
            .Bind(configuration.GetSection(TelegramReminderOptions.SectionName));
        services.AddSingleton<ChatIdParser>();
        services.AddSingleton<TimeZoneResolver>();
        services.AddSingleton<TelegramReminderConfigBuilder>();
        services.AddHostedService<TelegramReminderService>();

        return services;
    }
}
