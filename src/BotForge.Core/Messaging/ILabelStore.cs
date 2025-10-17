namespace BotForge.Messaging;

/// <summary>
/// Provides localized button labels by key.
/// </summary>
public interface ILabelStore
{
    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for the specified <paramref name="key"/>.
    /// Implementations should throw or return a fallback label when the key is not found.
    /// </summary>
    /// <param name="key">The label key.</param>
    /// <returns>The corresponding <see cref="ButtonLabel"/>.</returns>
    ButtonLabel GetLabel(string key);
}
