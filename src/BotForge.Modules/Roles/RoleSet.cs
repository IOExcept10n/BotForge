using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace BotForge.Modules.Roles;

/// <summary>
/// Represents a set of roles, allowing listing of role permissions.
/// </summary>
public class RoleSet : ICollection<Role>
{
    private bool _allowAll;
    private FrozenSet<Role> _roles;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleSet"/> class.
    /// </summary>
    public RoleSet()
    {
        _roles = FrozenSet.Create<Role>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleSet"/> class with a specified set of roles.
    /// </summary>
    /// <param name="roles">The roles to initialize the set with.</param>
    public RoleSet(params ReadOnlySpan<Role> roles)
    {
        InitializeRoles(roles);
    }

    /// <summary>
    /// Gets a <see cref="RoleSet"/> instance that allows all roles.
    /// </summary>
    public static RoleSet AllowAll => new() { _allowAll = true };

    /// <inheritdoc/>
    public int Count => _roles.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(Role role)
    {
        if (_allowAll)
        {
            var currentBlacklist = _roles.ToHashSet();
            currentBlacklist.Remove(role);
            _roles = currentBlacklist.ToFrozenSet();
        }
        else
        {
            var currentWhitelist = _roles.ToHashSet();
            var add = currentWhitelist.Add(role);
            _roles = currentWhitelist.ToFrozenSet();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _roles = FrozenSet.Create<Role>();
    }

    /// <inheritdoc/>
    public bool Contains(Role role)
    {
        return _allowAll ? !_roles.Contains(role) : _roles.Contains(role);
    }

    /// <inheritdoc/>
    public void CopyTo(Role[] array, int arrayIndex)
    {
        _roles.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<Role> GetEnumerator() => _roles.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Initializes the roles in the set with the specified set of roles.
    /// </summary>
    /// <param name="roles">The roles to initialize with.</param>
    [MemberNotNull(nameof(_roles))]
    public void InitializeRoles(params ReadOnlySpan<Role> roles) => _roles = FrozenSet.Create(roles);

    /// <inheritdoc/>
    public bool Remove(Role role)
    {
        if (_allowAll)
        {
            var currentBlacklist = _roles.ToHashSet();
            bool add = currentBlacklist.Add(role);
            _roles = currentBlacklist.ToFrozenSet();
            return add;
        }
        else
        {
            if (!_roles.Contains(role)) return false;
            var currentWhitelist = _roles.ToHashSet();
            bool remove = currentWhitelist.Remove(role);
            _roles = currentWhitelist.ToFrozenSet();
            return remove;
        }
    }
}
