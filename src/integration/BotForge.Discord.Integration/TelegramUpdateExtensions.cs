using BotForge.Messaging;
using Telegram.Bot.Types;

namespace BotForge.Telegram.Integration;

public static class TelegramUpdateExtensions
{
    /// <summary>
    /// Converts a Telegram <see cref="Update"/> to a BotForge <see cref="IUpdate"/> using <see cref="TelegramUpdateParser"/>.
    /// </summary>
    public static IUpdate ToBotForge(this Update update)
        => TelegramUpdateParser.Parse(update);
}
