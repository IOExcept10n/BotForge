using System.Globalization;

namespace BotForge.Messaging;

/// <summary>
/// Identifies a user across transports and provides optional metadata used by the FSM.
/// </summary>
/// <param name="Id">Numeric user identifier.</param>
/// <param name="Username">Optional username.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Discriminator">Optional numeric discriminator (platform-specific).</param>
/// <param name="Locale">Optional user locale used for localization.</param>
public record UserIdentity(long Id, string? Username = null, string? DisplayName = null, int Discriminator = 0, CultureInfo? Locale = null);
