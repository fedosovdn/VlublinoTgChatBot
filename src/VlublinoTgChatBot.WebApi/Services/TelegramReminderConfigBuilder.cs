using Cronos;
using Microsoft.Extensions.Options;
using VlublinoTgChatBot.WebApi.Options;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TelegramReminderConfigBuilder
{
    private readonly ILogger<TelegramReminderConfigBuilder> _logger;
    private readonly IOptionsMonitor<TelegramReminderOptions> _optionsMonitor;
    private readonly IOptionsMonitor<TelegramBotOptions> _botOptionsMonitor;
    private readonly ChatIdParser _chatIdParser;
    private readonly TimeZoneResolver _timeZoneResolver;

    public TelegramReminderConfigBuilder(
        ILogger<TelegramReminderConfigBuilder> logger,
        IOptionsMonitor<TelegramReminderOptions> optionsMonitor,
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

    public bool TryBuild(out ReminderConfig config)
    {
        config = null!;

        var options = _optionsMonitor.CurrentValue;
        var botOptions = _botOptionsMonitor.CurrentValue;
        if (string.IsNullOrWhiteSpace(botOptions.BotToken))
        {
            _logger.LogWarning("TG_BOT_TOKEN не задан. Сервис напоминаний отключен.");
            return false;
        }

        var chatIds = _chatIdParser.Parse(botOptions.ChatIds);
        if (chatIds.Count == 0)
        {
            _logger.LogWarning("TG_CHAT_IDS пуст. Сервис напоминаний отключен.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.ScheduleCron))
        {
            _logger.LogWarning("TG_SCHEDULE_CRON не задан. Сервис напоминаний отключен.");
            return false;
        }

        if (!CronExpression.TryParse(options.ScheduleCron, CronFormat.Standard, out var schedule))
        {
            _logger.LogWarning("TG_SCHEDULE_CRON имеет неверный формат: {Cron}.", options.ScheduleCron);
            return false;
        }

        var timeZone = TimeZoneInfo.Utc;
        if (!string.IsNullOrWhiteSpace(options.TimeZoneId))
        {
            if (!_timeZoneResolver.TryResolve(options.TimeZoneId, out timeZone))
            {
                _logger.LogWarning("TG_TIMEZONE не найден: {TimeZone}. Используется UTC.", options.TimeZoneId);
                timeZone = TimeZoneInfo.Utc;
            }
        }

        var message = string.IsNullOrWhiteSpace(options.Message)
            ? "Time to pay rent."
            : options.Message;

        config = new ReminderConfig(
            botOptions.BotToken,
            chatIds,
            schedule,
            timeZone,
            message);

        return true;
    }
}
