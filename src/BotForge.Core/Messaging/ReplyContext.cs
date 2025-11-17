namespace BotForge.Messaging;

/// <summary>
/// Context describing a reply to send to a user, including optional text and an optional reply keyboard.
/// </summary>
/// <param name="Message">The textual message to send. If null, no message body is sent and only keyboard actions may be applied.</param>
/// <param name="Keyboard">Optional <see cref="ReplyKeyboard"/> describing buttons to present to the user.</param>
public record ReplyContext(string? Message, ReplyKeyboard? Keyboard);

/// <summary>
/// Represents a keyboard layout composed of rows of button labels.
/// Each inner <see cref="IEnumerable{T}"/> represents a row; the outer <see cref="IEnumerable{T}"/> represents the full keyboard.
/// </summary>
/// <param name="Buttons">Rows of <see cref="ButtonLabel"/> that make up the keyboard.</param>
public record ReplyKeyboard(IEnumerable<IEnumerable<ButtonLabel>> Buttons)
{
    /// <summary>
    /// Gets or sets a value indicating whether the keyboard should be removed after first interaction.
    /// </summary>
    public bool OneTime { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the keyboard should be resized depending on content.
    /// </summary>
    public bool Resize { get; set; } = true;
}
