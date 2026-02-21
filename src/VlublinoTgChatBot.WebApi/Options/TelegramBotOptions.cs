namespace VlublinoTgChatBot.WebApi.Options;

public sealed class TelegramBotOptions
{
    public const string SectionName = "TelegramBot";

    public string? BotToken { get; set; }
    public string? ChatIds { get; set; }
}
