using System.Diagnostics.CodeAnalysis;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the state context for a module, containing user and chat information,
/// the user's role, the current message, and the current state.
/// </summary>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="UserRole">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record ModuleStateContext(UserIdentity User,
                                 ChatId Chat,
                                 Role UserRole,
                                 IMessage Message,
                                 StateRecord CurrentState,
                                 IServiceProvider Services) : RoleStateContext(User, UserRole, Message, CurrentState, Services)
{
    /// <summary>
    /// Converts the instance of the <see cref="ModuleStateContext"/> to <see cref="SelectionStateContext"/> by passing the set of buttons that provide menu options.
    /// </summary>
    /// <param name="selectionButtons">A set of buttons to select from which.</param>
    /// <returns>An instance of the <see cref="SelectionStateContext"/> that contains info about provided buttons.</returns>
    public SelectionStateContext ToSelectionContext(IEnumerable<(string Name, ButtonLabel Button)> selectionButtons) => new(User, Chat, UserRole, selectionButtons, Message, CurrentState, Services);

    /// <summary>
    /// Tries to convert user input to <see cref="PromptStateContext{T}"/>, based on input options.
    /// </summary>
    /// <typeparam name="T">Type of the input value.</typeparam>
    /// <param name="allowTextInput">A flag indicating whether the state allows text message input.</param>
    /// <param name="allowFileInput">A flag indicating whether the state allows file message input.</param>
    /// <param name="context">A resulting context that contains info about input value of a specific type.</param>
    /// <returns><see langword="true"/> if prompted value is correct and conversion succeeded; otherwise <see langword="false"/>.</returns>
    public bool TryToPromptContext<T>(bool allowTextInput, bool allowFileInput, [NotNullWhen(true)] out PromptStateContext<T>? context) where T : IParsable<T>
    {
        context = null;

        (Optional<T> data, bool invalidContent) = Message.Content switch
        {
            TextMessageContent text when allowTextInput && T.TryParse(text.Text, null, out var d) => (d, false),
            FileMessageContent when allowFileInput => (Optional<T>.None, false),
            _ => (Optional<T>.None, true),
        };

        if (invalidContent)
            return false;

        context = new(User, Chat, UserRole, Message, data, CurrentState, Services);
        return true;
    }
}
