using System.Diagnostics.CodeAnalysis;

namespace BotForge;

/// <summary>
/// Provides registration and lookup of items of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the registered item.</typeparam>
public interface IRegistry<T> where T : class
{
    /// <summary>
    /// Registers a new instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">An item to register.</param>
    void Register(T instance);

    /// <summary>
    /// Attempts to retrieve an item by its identifying key.
    /// </summary>
    /// <param name="key">The key to retrieve an item.</param>
    /// <param name="instance">When successful, contains the instance of <typeparamref name="T"/>; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the state was found; otherwise, <see langword="false"/>.</returns>
    bool TryGet(string key, [NotNullWhen(true)] out T? instance);
}
