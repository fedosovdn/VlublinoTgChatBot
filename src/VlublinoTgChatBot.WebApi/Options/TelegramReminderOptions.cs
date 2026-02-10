namespace VlublinoTgChatBot.WebApi.Options;

public sealed class TelegramReminderOptions
{
    public const string SectionName = "TelegramReminder";

    public string? BotToken { get; set; }
    public string? ChatIds { get; set; }
    public string? ScheduleCron { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Message { get; set; }
}
