using Telegram.Bot.Types;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed record MonthEndReminderConfig(
    string BotToken,
    IReadOnlyList<ChatId> ChatIds,
    TimeZoneInfo TimeZone,
    string Message,
    TimeSpan SendAtLocalTime);
