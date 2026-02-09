using Cronos;
using Telegram.Bot.Types;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed record ReminderConfig(
    string BotToken,
    IReadOnlyList<ChatId> ChatIds,
    CronExpression Schedule,
    TimeZoneInfo TimeZone,
    string Message);
