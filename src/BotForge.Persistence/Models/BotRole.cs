using System;
using System.Collections.Generic;
using System.Text;

namespace BotForge.Persistence.Models;

public class BotRole
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? WelcomeMessageKey { get; set; }
}
