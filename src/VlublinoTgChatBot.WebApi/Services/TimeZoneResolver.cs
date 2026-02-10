using System.Runtime.InteropServices;

namespace VlublinoTgChatBot.WebApi.Services;

internal sealed class TimeZoneResolver
{
    private static readonly IReadOnlyDictionary<string, string> TimeZoneFallbacks =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Europe/Moscow"] = "Russian Standard Time"
        };

    public bool TryResolve(string timeZoneId, out TimeZoneInfo timeZone)
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out timeZone))
        {
            return true;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            && TimeZoneFallbacks.TryGetValue(timeZoneId, out var fallbackId)
            && TimeZoneInfo.TryFindSystemTimeZoneById(fallbackId, out timeZone))
        {
            return true;
        }

        timeZone = TimeZoneInfo.Utc;
        return false;
    }
}
