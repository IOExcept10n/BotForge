using BotForge.Modules.Roles;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

/// <summary>
/// Represents a repository of roles defined in BotForge bot app.
/// </summary>
public interface IBotRoleRepository : IRepository<long, BotRole>
{
    /// <summary>
    /// Registers role in database by creating a new entity or retrieving an old one.
    /// </summary>
    /// <param name="role">A regular registered role to get DB representation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel indexing task.</param>
    /// <returns>An asynchronous task that retrieves DB representation of the role.</returns>
    Task<BotRole> RegisterAsync(Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Gets role record in database by its regular representation.
    /// </summary>
    /// <remarks>
    /// The difference of this method from <see cref="RegisterAsync(Role, CancellationToken)"/> is that this method fails when there's no roles found by specified role.
    /// </remarks>
    /// <param name="role">The regular role to get DB entity for.</param>
    /// <param name="cancellationToken">A cancellation token to cancel indexing task.</param>
    /// <returns>An asynchronous task that retrieves DB representation of the role.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there are no roles defined in DB for the given role instance.</exception>
    Task<BotRole> GetRoleAsync(Role role, CancellationToken cancellationToken);
}
