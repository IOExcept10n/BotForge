using BotForge.Localization;
using BotForge.Messaging;

namespace BotForge.Fsm;

/// <summary>
/// Represents a state-specific layout that can send a message (for example, a prompt or UI) to a user.
/// </summary>
public interface IStateLayout
{
    /// <summary>
    /// Sends the layout message to the specified <paramref name="channel"/> and <paramref name="user"/>.
    /// Implementations should use <paramref name="overrideMessage"/> when provided to alter or replace the default message content.
    /// </summary>
    /// <param name="channel">The reply channel to use for sending the message. Cannot be null.</param>
    /// <param name="user">The user to whom the message will be sent.</param>
    /// <param name="localization">An instance of the localization service to localize output message.</param>
    /// <param name="overrideMessage">Optional context to override or augment the default reply message.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendLayoutMessageAsync(IReplyChannel channel, UserIdentity user, ILocalizationService localization, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default);
}
