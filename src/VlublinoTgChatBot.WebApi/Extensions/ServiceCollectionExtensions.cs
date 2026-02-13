using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            .Bind(configuration.GetSection(TelegramReminderOptions.SectionName))
            .PostConfigure(options =>
            {
                var botToken = configuration["TG_BOT_TOKEN"];
                if (!string.IsNullOrWhiteSpace(botToken))
                {
                    options.BotToken = botToken;
                }

                var chatIds = configuration["TG_CHAT_IDS"];
                if (!string.IsNullOrWhiteSpace(chatIds))
                {
                    options.ChatIds = chatIds;
                }

                var scheduleCron = configuration["TG_SCHEDULE_CRON"];
                if (!string.IsNullOrWhiteSpace(scheduleCron))
                {
                    options.ScheduleCron = scheduleCron;
                }

                var timeZoneId = configuration["TG_TIMEZONE"];
                if (!string.IsNullOrWhiteSpace(timeZoneId))
                {
                    options.TimeZoneId = timeZoneId;
                }

                var message = configuration["TG_MESSAGE"];
                if (!string.IsNullOrWhiteSpace(message))
                {
                    options.Message = message;
                }
            });
        services.AddSingleton<ChatIdParser>();
        services.AddSingleton<TimeZoneResolver>();
        services.AddSingleton<TelegramReminderConfigBuilder>();
        services.AddHostedService<TelegramReminderService>();

        return services;
    }
}
