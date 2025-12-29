namespace BotForge.Modules.Attributes;

/// <summary>
/// Defines a custom state that can handle any user message.
/// </summary>
/// <param name="promptLocalizationKey">A key for the state localized prompt message.</param>
public sealed class CustomStateAttribute(string promptLocalizationKey) : FsmStateAttribute(promptLocalizationKey);
