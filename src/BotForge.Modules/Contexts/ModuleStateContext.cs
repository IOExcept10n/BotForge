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
    public SelectionStateContext ToSelectionContext(IEnumerable<(string Name, ButtonLabel Button)> selectionButtons) => new(User, Chat, UserRole, selectionButtons, Message, CurrentState, Services);

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
