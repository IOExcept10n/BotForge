namespace BotForge.Messaging;

/// <summary>
/// Provides localized button labels by key.
/// </summary>
public interface ILabelStore
{
    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for 'Back' predefined action.
    /// </summary>
    ButtonLabel BackButton { get; }

    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for 'Cancel' predefined action.
    /// </summary>
    ButtonLabel CancelButton { get; }

    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for 'Ok' predefined action.
    /// </summary>
    ButtonLabel OkButton { get; }

    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for 'No' predefined option.
    /// </summary>
    ButtonLabel NoButton { get; }

    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for 'Yes' predefined option.
    /// </summary>
    ButtonLabel YesButton { get; }

    /// <summary>
    /// Gets a <see cref="ButtonLabel"/> for the specified <paramref name="key"/>.
    /// Implementations should throw or return a fallback label when the key is not found.
    /// </summary>
    /// <param name="key">The label key.</param>
    /// <returns>The corresponding <see cref="ButtonLabel"/>.</returns>
    ButtonLabel GetLabel(string key);
}
