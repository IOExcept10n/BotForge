using System.Collections;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace BotForge.Modules.Roles;

public class RoleCatalog : ICollection<Role>
{
    private FrozenSet<Role> _roles;
    private bool _allowAll;

    public RoleCatalog()
    {
        _roles = FrozenSet.Create<Role>();
    }

    public RoleCatalog(params ReadOnlySpan<Role> roles)
    {
        InitializeRoles(roles);
    }

    public int Count => _roles.Count;

    public bool IsReadOnly => false;

    public static RoleCatalog AllowAll => new() { _allowAll = true };

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

    public void Clear()
    {
        _roles = FrozenSet.Create<Role>();
    }

    public bool Contains(Role role)
    {
        return _allowAll ? !_roles.Contains(role) : _roles.Contains(role);
    }

    public void CopyTo(Role[] array, int arrayIndex)
    {
        _roles.CopyTo(array, arrayIndex);
    }

    public bool Remove(Role role)
    {
        if (_allowAll)
        {
            var currentBlacklist = _roles.ToHashSet();
            bool add = currentBlacklist.Add(role); // Добавляем роль в blacklist
            _roles = currentBlacklist.ToFrozenSet();
            return add;
        }
        else
        {
            if (!_roles.Contains(role)) return false;
            var currentWhitelist = _roles.ToHashSet();
            bool remove = currentWhitelist.Remove(role); // Убираем роль из whitelist
            _roles = currentWhitelist.ToFrozenSet();
            return remove;
        }
    }

    public IEnumerator<Role> GetEnumerator() => _roles.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void InitializeRoles(params ReadOnlySpan<Role> roles) { _roles = FrozenSet.Create(roles); }
}

