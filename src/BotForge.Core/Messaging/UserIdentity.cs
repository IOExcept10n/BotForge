using System.Globalization;

namespace BotForge.Messaging;

public record UserIdentity(long Id, string? Username = null, string? DisplayName = null, int Discriminator = 0, CultureInfo? Locale = null);
