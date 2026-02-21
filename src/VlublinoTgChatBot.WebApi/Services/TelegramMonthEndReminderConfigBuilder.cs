using Microsoft.Extensions.Options;
using VlublinoTgChatBot.WebApi.Options;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramMonthEndReminderConfigBuilder
{
    private readonly ILogger<TelegramMonthEndReminderConfigBuilder> _logger;
    private readonly IOptionsMonitor<TelegramMonthEndReminderOptions> _optionsMonitor;
    private readonly IOptionsMonitor<TelegramBotOptions> _botOptionsMonitor;
    private readonly ChatIdParser _chatIdParser;
    private readonly TimeZoneResolver _timeZoneResolver;

    public TelegramMonthEndReminderConfigBuilder(
        ILogger<TelegramMonthEndReminderConfigBuilder> logger,
        IOptionsMonitor<TelegramMonthEndReminderOptions> optionsMonitor,
        IOptionsMonitor<TelegramBotOptions> botOptionsMonitor,
        ChatIdParser chatIdParser,
        TimeZoneResolver timeZoneResolver)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
        _botOptionsMonitor = botOptionsMonitor;
        _chatIdParser = chatIdParser;
        _timeZoneResolver = timeZoneResolver;
    }

    public bool TryBuild(out MonthEndReminderConfig config)
    {
        config = null!;

        var options = _optionsMonitor.CurrentValue;
        var botOptions = _botOptionsMonitor.CurrentValue;
        if (string.IsNullOrWhiteSpace(botOptions.BotToken))
        {
            _logger.LogWarning("TG_BOT_TOKEN не задан. Сервис месячных напоминаний отключен.");
            return false;
        }

        var chatIds = _chatIdParser.Parse(botOptions.ChatIds);
        if (chatIds.Count == 0)
        {
            _logger.LogWarning("TG_CHAT_IDS пуст. Сервис месячных напоминаний отключен.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            _logger.LogWarning("TG_MONTHEND_MESSAGE не задан. Сервис месячных напоминаний отключен.");
            return false;
        }

        if (!TryParseSendAtLocalTime(options, out var sendAtLocalTime))
        {
            return false;
        }

        var timeZone = TimeZoneInfo.Utc;
        if (!string.IsNullOrWhiteSpace(options.TimeZoneId))
        {
            if (!_timeZoneResolver.TryResolve(options.TimeZoneId, out timeZone))
            {
                _logger.LogWarning("TG_MONTHEND_TIMEZONE не найден: {TimeZone}. Используется UTC.", options.TimeZoneId);
                timeZone = TimeZoneInfo.Utc;
            }
        }

        config = new MonthEndReminderConfig(
            botOptions.BotToken,
            chatIds,
            timeZone,
            options.Message,
            sendAtLocalTime);

        return true;
    }

    private bool TryParseSendAtLocalTime(
        TelegramMonthEndReminderOptions options,
        out TimeSpan sendAtLocalTime)
    {
        sendAtLocalTime = default;
        if (string.IsNullOrWhiteSpace(options.SendAtLocalTime))
        {
            _logger.LogWarning("TG_MONTHEND_SEND_AT_LOCAL_TIME не задан. Сервис месячных напоминаний отключен.");
            return false;
        }

        if (TimeSpan.TryParse(options.SendAtLocalTime, out sendAtLocalTime))
        {
            return true;
        }

        _logger.LogWarning(
            "TG_MONTHEND_SEND_AT_LOCAL_TIME имеет неверный формат: {SendAt}. Сервис месячных напоминаний отключен.",
            options.SendAtLocalTime);
        return false;
    }
}
