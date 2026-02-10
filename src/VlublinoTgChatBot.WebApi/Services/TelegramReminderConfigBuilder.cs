using Cronos;
using Microsoft.Extensions.Options;
using VlublinoTgChatBot.WebApi.Options;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramReminderConfigBuilder
{
    private readonly ILogger<TelegramReminderConfigBuilder> _logger;
    private readonly IOptionsMonitor<TelegramReminderOptions> _optionsMonitor;
    private readonly ChatIdParser _chatIdParser;
    private readonly TimeZoneResolver _timeZoneResolver;

    public TelegramReminderConfigBuilder(
        ILogger<TelegramReminderConfigBuilder> logger,
        IOptionsMonitor<TelegramReminderOptions> optionsMonitor,
        ChatIdParser chatIdParser,
        TimeZoneResolver timeZoneResolver)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
        _chatIdParser = chatIdParser;
        _timeZoneResolver = timeZoneResolver;
    }

    public bool TryBuild(out ReminderConfig config)
    {
        config = default!;

        var options = _optionsMonitor.CurrentValue;
        if (string.IsNullOrWhiteSpace(options.BotToken))
        {
            _logger.LogWarning("TG_BOT_TOKEN is not set. Reminder service is disabled.");
            return false;
        }

        var chatIds = _chatIdParser.Parse(options.ChatIds);
        if (chatIds.Count == 0)
        {
            _logger.LogWarning("TG_CHAT_IDS is empty. Reminder service is disabled.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.ScheduleCron))
        {
            _logger.LogWarning("TG_SCHEDULE_CRON is not set. Reminder service is disabled.");
            return false;
        }

        if (!CronExpression.TryParse(options.ScheduleCron, CronFormat.Standard, out var schedule))
        {
            _logger.LogWarning("TG_SCHEDULE_CRON has invalid format: {Cron}.", options.ScheduleCron);
            return false;
        }

        var timeZone = TimeZoneInfo.Utc;
        if (!string.IsNullOrWhiteSpace(options.TimeZoneId))
        {
            if (!_timeZoneResolver.TryResolve(options.TimeZoneId, out timeZone))
            {
                _logger.LogWarning("TG_TIMEZONE not found: {TimeZone}. Using UTC.", options.TimeZoneId);
                timeZone = TimeZoneInfo.Utc;
            }
        }

        var message = string.IsNullOrWhiteSpace(options.Message)
            ? "Time to pay rent."
            : options.Message;

        config = new ReminderConfig(
            options.BotToken,
            chatIds,
            schedule,
            timeZone,
            message);

        return true;
    }
}
