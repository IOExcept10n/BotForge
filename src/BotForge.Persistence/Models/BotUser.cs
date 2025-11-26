namespace BotForge.Persistence.Models;

public class BotUser
{
    public Guid Id { get; set; }

    public long RoleId { get; set; }

    public long PlatformUserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public int Discriminator { get; set; }

    public string PlatformName { get; set; } = string.Empty;

    public string? PreferredLocale { get; set; }

    public string? OriginalLocale { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastSeen { get; set; }

    public UserState? State { get; set; }

    public BotRole? Role { get; set; }
}
