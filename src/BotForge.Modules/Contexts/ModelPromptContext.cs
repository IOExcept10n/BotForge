using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the context for a model prompt, including a model that's been input by user.
/// </summary>
/// <typeparam name="TModel">The type of the model being manipulated.</typeparam>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="Role">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="Model">The model instance being worked on.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record ModelPromptContext<TModel>(
    UserIdentity User,
    ChatId Chat,
    Role Role,
    IMessage Message,
    TModel Model,
    StateRecord CurrentState,
    IServiceProvider Services) :
    ModuleStateContext(User, Chat, Role, Message, CurrentState, Services)
    where TModel : new();
