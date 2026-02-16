using Telegram.Bot;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramReminderService : BackgroundService
{
    private readonly ILogger<TelegramReminderService> _logger;
    private readonly TelegramReminderConfigBuilder _configBuilder;

    public TelegramReminderService(
        ILogger<TelegramReminderService> logger,
        TelegramReminderConfigBuilder configBuilder)
    {
        _logger = logger;
        _configBuilder = configBuilder;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configBuilder.TryBuild(out var config))
        {
            return;
        }

        var botClient = new TelegramBotClient(config.BotToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextUtc = config.Schedule.GetNextOccurrence(DateTimeOffset.UtcNow, config.TimeZone);
            if (nextUtc is null)
            {
                _logger.LogWarning("Для расписания cron в будущем не будет никаких событий");
                break;
            }

            var delay = nextUtc.Value - DateTimeOffset.UtcNow;
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            await Task.Delay(delay, stoppingToken);

            foreach (var chatId in config.ChatIds)
            {
                try
                {
                    await botClient.SendMessage(chatId, config.Message, cancellationToken: stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Не удалось отправить напоминание в чат {ChatId}", chatId);
                }
            }
        }
    }
}
