using Telegram.Bot;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramMonthEndReminderService : BackgroundService
{
    private readonly ILogger<TelegramMonthEndReminderService> _logger;
    private readonly TelegramMonthEndReminderConfigBuilder _configBuilder;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TelegramMonthEndReminderService(
        ILogger<TelegramMonthEndReminderService> logger,
        TelegramMonthEndReminderConfigBuilder configBuilder,
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
            var nextUtc = GetNextMonthEndUtc(nowUtc, config.TimeZone, config.SendAtLocalTime);
            var delay = nextUtc - nowUtc;
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
                    _logger.LogError(ex, "Не удалось отправить месячное напоминание в чат {ChatId}", chatId);
                }
            }
        }
    }

    private static DateTimeOffset GetNextMonthEndUtc(
        DateTimeOffset nowUtc,
        TimeZoneInfo timeZone,
        TimeSpan sendAtLocalTime)
    {
        var nowLocal = TimeZoneInfo.ConvertTime(nowUtc, timeZone);
        var lastDayOfMonth = DateTime.DaysInMonth(nowLocal.Year, nowLocal.Month);
        var candidateLocal = new DateTime(
            nowLocal.Year,
            nowLocal.Month,
            lastDayOfMonth,
            sendAtLocalTime.Hours,
            sendAtLocalTime.Minutes,
            sendAtLocalTime.Seconds,
            DateTimeKind.Unspecified);

        var candidateLocalOffset = new DateTimeOffset(candidateLocal, timeZone.GetUtcOffset(candidateLocal));
        if (nowLocal <= candidateLocalOffset)
        {
            return candidateLocalOffset.ToUniversalTime();
        }

        var nextMonth = nowLocal.AddMonths(1);
        var nextLastDay = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
        var nextLocal = new DateTime(
            nextMonth.Year,
            nextMonth.Month,
            nextLastDay,
            sendAtLocalTime.Hours,
            sendAtLocalTime.Minutes,
            sendAtLocalTime.Seconds,
            DateTimeKind.Unspecified);

        var nextLocalOffset = new DateTimeOffset(nextLocal, timeZone.GetUtcOffset(nextLocal));
        return nextLocalOffset.ToUniversalTime();
    }
}
