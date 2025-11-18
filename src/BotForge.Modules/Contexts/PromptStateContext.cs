using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the prompt state context, allowing for user input of type T.
/// </summary>
/// <typeparam name="T">The type of input expected from the user.</typeparam>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="Role">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="Input">The optional input provided by the user.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record PromptStateContext<T>(
    UserIdentity User,
    ChatId Chat,
    Role Role,
    IMessage Message,
    Optional<T> Input,
    StateRecord CurrentState,
    IServiceProvider Services) :
    ModuleStateContext(User, Chat, Role, Message, CurrentState, Services)
    where T : IParsable<T>;
