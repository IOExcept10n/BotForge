namespace BotForge.Persistence.Models;

/// <summary>
/// Represents a DB entity model for the BotForge user FSM state details.
/// </summary>
public class UserState
{
    /// <summary>
    /// Gets or sets the unique identifier for the user state.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user this state belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique text identifier for the state itself.
    /// </summary>
    public required string StateId { get; set; }

    /// <summary>
    /// Gets or sets optional additional data related to the user’s state.
    /// </summary>
    public string? StateData { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating when the user state was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the associated user for this state.
    /// </summary>
    public BotUser User { get; set; } = null!;
}
