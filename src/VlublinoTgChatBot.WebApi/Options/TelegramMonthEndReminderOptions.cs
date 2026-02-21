namespace VlublinoTgChatBot.WebApi.Options;

public sealed class TelegramMonthEndReminderOptions
{
    public const string SectionName = "TelegramMonthEndReminder";

    public string? TimeZoneId { get; set; }
    public string? Message { get; set; }
    public string? SendAtLocalTime { get; set; }
}
