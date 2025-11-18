using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the context for a role-related state, including user identity and message information.
/// </summary>
/// <param name="User">The identity of the user associated with this context.</param>
/// <param name="UserRole">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record RoleStateContext(UserIdentity User, Role UserRole, IMessage Message, StateRecord CurrentState, IServiceProvider Services) : MessageStateContext(Message, CurrentState, Services)
{
    /// <summary>
    /// Creates a new instance of <see cref="RoleStateContext"/> from an existing <see cref="MessageStateContext"/> and a specified role.
    /// </summary>
    /// <param name="ctx">The message state context to extract information from.</param>
    /// <param name="role">The role to associate with the new context.</param>
    /// <returns>A new instance of <see cref="RoleStateContext"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ctx"/> is null.</exception>
    public static RoleStateContext FromContext(MessageStateContext ctx, Role role)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return new(ctx.Message.From, role, ctx.Message, ctx.CurrentState, ctx.Services);
    }
}
