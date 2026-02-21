using VlublinoTgChatBot.WebApi.Options;
using VlublinoTgChatBot.WebApi.Services;

namespace VlublinoTgChatBot.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TelegramBotOptions>()
            .Bind(configuration.GetSection(TelegramBotOptions.SectionName));
        services.AddOptions<TelegramReminderOptions>()
            .Bind(configuration.GetSection(TelegramReminderOptions.SectionName));
        services.AddOptions<TelegramMonthEndReminderOptions>()
            .Bind(configuration.GetSection(TelegramMonthEndReminderOptions.SectionName));
        services.AddSingleton<ChatIdParser>();
        services.AddSingleton<TimeZoneResolver>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<TelegramReminderConfigBuilder>();
        services.AddSingleton<TelegramMonthEndReminderConfigBuilder>();
        services.AddHostedService<TelegramReminderService>();
        services.AddHostedService<TelegramMonthEndReminderService>();

        return services;
    }
}
