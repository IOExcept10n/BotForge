using System.ComponentModel.DataAnnotations;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the intermediate context for model binding, facilitating the binding of user input
/// to a model based on predefined descriptors.
/// </summary>
/// <remarks>
/// This context is handled internally, and you should not write custom handlers targeting such context.
/// However, for example, you can override method <see cref="ModuleBase.ValidateModelAsync(ModelBindContext, ModelBindingBuilder, ICollection{ValidationResult})"/> that relies on this context,
/// to customize model binding process.
/// </remarks>
/// <typeparam name="T">The type of model being bound.</typeparam>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="Role">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="Binding">The binding descriptor containing model metadata.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record ModelBindContext(
    UserIdentity User,
    ChatId Chat,
    Role Role,
    IMessage Message,
    ModelBindingDescriptor Binding,
    StateRecord CurrentState,
    IServiceProvider Services) :
    ModuleStateContext(User, Chat, Role, Message, CurrentState, Services);
