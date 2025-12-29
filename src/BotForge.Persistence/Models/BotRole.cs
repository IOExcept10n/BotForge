namespace BotForge.Persistence.Models;

/// <summary>
/// Represents a DB entity model for the BotForge user role.
/// </summary>
public class BotRole
{
    /// <summary>
    /// Gets or sets the unique identifier for the role.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// This property is initialized to an empty string.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the optional key for a welcome message associated with the role.
    /// </summary>
    public string? WelcomeMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the collection of users associated with the bot role.
    /// </summary>
    public ICollection<BotUser> Users { get; init; } = [];
}
