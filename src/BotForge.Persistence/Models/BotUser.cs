namespace BotForge.Persistence.Models;

/// <summary>
/// Represents a DB entity model for the BotForge user.
/// </summary>
public class BotUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the bot user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the optional identifier for the user's role.
    /// </summary>
    public long? RoleId { get; set; }

    /// <summary>
    /// Gets or sets the optional identifier for the platform-specific user account.
    /// </summary>
    public long? PlatformUserId { get; set; }

    /// <summary>
    /// Gets or sets the username of the bot user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discriminator for the user.
    /// </summary>
    public int Discriminator { get; set; }

    /// <summary>
    /// Gets or sets the display name of the bot user.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional preferred locale for the user.
    /// </summary>
    public string? PreferredLocale { get; set; }

    /// <summary>
    /// Gets or sets the optional original locale for the user.
    /// </summary>
    public string? OriginalLocale { get; set; }

    /// <summary>
    /// Gets or sets the timestamp representing when the bot user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last time the bot user interacted with bot.
    /// </summary>
    public DateTimeOffset LastSeen { get; set; }

    /// <summary>
    /// Gets or sets the current state of the user.
    /// </summary>
    public UserState? State { get; set; }

    /// <summary>
    /// Gets or sets the associated role for the bot user.
    /// </summary>
    public BotRole? Role { get; set; }
}
