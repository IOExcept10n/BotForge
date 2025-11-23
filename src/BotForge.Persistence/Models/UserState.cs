using System;
using System.Collections.Generic;
using System.Text;

namespace BotForge.Persistence.Models;

public class UserState
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string StateId { get; set; }

    public string? StateData { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public BotUser User { get; set; } = null!;
}
