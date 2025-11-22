using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Modules.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules.Handlers;

/// <summary>
/// Represents a default implementation of /start command. Its behavior is to determine user role and then redirect him to the role main menu.
/// </summary>
public class StartCommandHandler : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => StateRecord.StartStateId;

    /// <inheritdoc/>
    public async Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default)
    {
        var roleService = ctx.Services.GetRequiredService<IRoleProvider>();
        var role = await roleService.GetRoleAsync(ctx.Interaction.From, cancellationToken).ConfigureAwait(false);
        var stateRegistry = ctx.Services.GetRequiredService<IRegistry<StateDefinition>>();
        string initialStateName = $"{role.Name}:{StateRecord.StartStateId}";
        if (!stateRegistry.TryGet(initialStateName, out _))
        {
            throw new InvalidOperationException("Couldn't determine initial state for the user role.");
        }
        return new(initialStateName);
    }
}
