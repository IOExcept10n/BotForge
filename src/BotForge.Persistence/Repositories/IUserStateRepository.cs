using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

/// <summary>
/// Represents a repository interface for managing user state entities.
/// </summary>
public interface IUserStateRepository : IRepository<Guid, UserState>
{
    /// <summary>
    /// Asynchronously retrieves a user state by the unique identifier of the associated user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose state is being retrieved.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the user state object if found; otherwise, <see langword="null"/>.</returns>
    Task<UserState?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
