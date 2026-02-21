namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}
