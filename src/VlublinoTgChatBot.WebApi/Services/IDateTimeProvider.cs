namespace VlublinoTgChatBot.WebApi.Services;

internal interface IDateTimeProvider
{
    DateTimeOffset GetUtcNow();
}
