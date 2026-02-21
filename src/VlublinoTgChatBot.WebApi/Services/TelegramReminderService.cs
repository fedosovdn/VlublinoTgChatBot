using Telegram.Bot;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramReminderService : BackgroundService
{
    private readonly ILogger<TelegramReminderService> _logger;
    private readonly TelegramReminderConfigBuilder _configBuilder;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TelegramReminderService(
        ILogger<TelegramReminderService> logger,
        TelegramReminderConfigBuilder configBuilder,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _configBuilder = configBuilder;
        _dateTimeProvider = dateTimeProvider;
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
            var nowUtc = _dateTimeProvider.GetUtcNow();
            var nextUtc = config.Schedule.GetNextOccurrence(nowUtc, config.TimeZone);
            if (nextUtc is null)
            {
                _logger.LogWarning("Для расписания cron в будущем не будет никаких событий");
                break;
            }

            var delay = nextUtc.Value - nowUtc;
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
