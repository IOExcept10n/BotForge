using System.Diagnostics.CodeAnalysis;

namespace BotForge.Fsm;

/// <summary>
/// Resolves <see cref="IStateHandler"/> instances by state identifier.
/// </summary>
public interface IStateHandlerResolver
{
    /// <summary>
    /// Attempts to resolve a handler for the specified <paramref name="stateId"/>.
    /// </summary>
    /// <param name="stateId">The state identifier to resolve. Cannot be null or empty.</param>
    /// <param name="handler">
    /// When this method returns, contains the resolved <see cref="IStateHandler"/> if the method returned <see langword="true"/>; otherwise <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if a handler was found for <paramref name="stateId"/>; otherwise <see langword="false"/>.</returns>
    bool TryResolve(string stateId, [NotNullWhen(true)] out IStateHandler? handler);
}
