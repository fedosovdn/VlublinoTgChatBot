using System.Globalization;
using Telegram.Bot.Types;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class ChatIdParser
{
    private readonly ILogger<ChatIdParser> _logger;

    public ChatIdParser(ILogger<ChatIdParser> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<ChatId> Parse(string? chatIdsValue)
    {
        var results = new List<ChatId>();
        if (string.IsNullOrWhiteSpace(chatIdsValue))
        {
            return results;
        }

        var parts = chatIdsValue.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (long.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                results.Add(new ChatId(id));
            }
            else
            {
                _logger.LogWarning("Skipping invalid chat id: {ChatId}.", part);
            }
        }

        return results;
    }
}
