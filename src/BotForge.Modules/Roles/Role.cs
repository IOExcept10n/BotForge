namespace BotForge.Modules.Roles;

/// <summary>
/// Represents a base class for roles with a specified name.
/// Provides equality comparison based on the role's name.
/// </summary>
/// <param name="Name">The name of the role.</param>
public abstract record Role(string Name)
{
    /// <summary>
    /// Gets a special instance of <see cref="Role"/> representing an unknown role.
    /// </summary>
    public static Role Unknown { get; } = new UnknownRole();

    /// <summary>
    /// Determines whether the specified <see cref="Role"/> is equal to the current <see cref="Role"/>.
    /// Comparison is based on the role's name.
    /// </summary>
    /// <param name="other">The role to compare with the current role.</param>
    /// <returns>true if the specified role is equal to the current role; otherwise, false.</returns>
    public virtual bool Equals(Role? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        return string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="Role"/> instance.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Role"/>.</returns>
    public override int GetHashCode() => Name?.GetHashCode(StringComparison.Ordinal) ?? 0;
}

/// <summary>
/// Represents an unknown role, which inherits from <see cref="Role"/>.
/// This role has an empty name.
/// </summary>
public sealed record UnknownRole() : Role(string.Empty);
