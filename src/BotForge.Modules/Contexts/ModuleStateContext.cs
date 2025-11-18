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
                                 IServiceProvider Services) : RoleStateContext(User, UserRole, Message, CurrentState, Services);
