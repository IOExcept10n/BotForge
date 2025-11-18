using System;
using System.Collections.Generic;
using System.Text;

namespace BotForge.Modules.Roles;

public abstract record Role(string Name)
{
    public static Role Unknown { get; } = new UnknownRole();

    public virtual bool Equals(Role? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        return string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override int GetHashCode() => Name?.GetHashCode(StringComparison.Ordinal) ?? 0;
}

public sealed record UnknownRole() : Role(string.Empty);
