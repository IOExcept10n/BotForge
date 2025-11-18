using System.Globalization;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Modules.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the state context for a selection, including buttons for user choices.
/// </summary>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="UserRole">The role of the user.</param>
/// <param name="SelectionButtons">The collection of buttons available for selection.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record SelectionStateContext(UserIdentity User,
                                    ChatId Chat,
                                    Role UserRole,
                                    IReadOnlyCollection<(string Name, ButtonLabel Button)> SelectionButtons,
                                    IMessage Message,
                                    StateRecord CurrentState,
                                    IServiceProvider Services) : ModuleStateContext(User, Chat, UserRole, Message, CurrentState, Services)
{
    /// <summary>
    /// Gets the name of the selected button based on the user's message input.
    /// </summary>
    /// <returns>The name of the selected button, or an empty string if none was selected.</returns>
    public string Selection()
    {
        if (Message is not TextMessageContent textMessage)
        {
            return string.Empty;
        }

        var localization = Services.GetRequiredService<ILocalizationService>();
        foreach (var (name, button) in SelectionButtons)
        {
            if (button.Localize(localization, User.Locale ?? CultureInfo.InvariantCulture) == textMessage.Text)
            {
                return name;
            }
        }

        return string.Empty;
    }
}
