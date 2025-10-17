using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BotForge.Localization;
using BotForge.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Fsm;

/// <summary>
/// Helper extensions for working with state contexts and state-related types.
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Attempts to deserialize the current state's <see cref="StateRecord.StateData"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize to.</typeparam>
    /// <param name="ctx">The <see cref="StateContext"/> containing the state's serialized data. May be <see langword="null"/>.</param>
    /// <param name="data">
    /// When this method returns, contains the deserialized value if the method returned <see langword="true"/>; otherwise the default value for <typeparamref name="T"/>.
    /// </param>
    /// <returns><see langword="true"/> if deserialization succeeded and a non-null value was produced; otherwise <see langword="false"/>.</returns>
    public static bool TryGetData<T>(this StateContext? ctx, [MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (string.IsNullOrWhiteSpace(ctx?.CurrentState.StateData))
            return false;

        try
        {
            var result = JsonSerializer.Deserialize<T>(ctx.CurrentState.StateData);
            if (result is null)
                return false;

            data = result;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the incoming message in <paramref name="ctx"/> matches the localized text of the provided <paramref name="label"/>.
    /// </summary>
    /// <param name="ctx">The message state context containing the incoming message and services. Cannot be <see langword="null"/> when called.</param>
    /// <param name="label">The button label to compare against. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the message is a text message and its text equals the localized label text; otherwise <see langword="false"/>.</returns>
    public static bool Matches(this MessageStateContext ctx, ButtonLabel label)
    {
        if (ctx == null || label == null)
            return false;
        if (ctx.Message.Content is not TextMessageContent msg)
            return false;
        string text = label.Localize(ctx.Services.GetRequiredService<ILocalizationService>(), ctx.Message.From.Locale ?? System.Globalization.CultureInfo.InvariantCulture);
        return msg.Text == text;
    }

    /// <summary>
    /// Produces a copy of the specified <paramref name="result"/> with its <see cref="StateResult.NextStateData"/>
    /// set to the serialized representation of <paramref name="data"/>.
    /// </summary>
    /// <typeparam name="TResult">A type derived from <see cref="StateResult"/>.</typeparam>
    /// <typeparam name="T">The type of the data to serialize into the state.</typeparam>
    /// <param name="result">The original state result. Cannot be null.</param>
    /// <param name="data">The data to serialize into the returned result's <see cref="StateResult.NextStateData"/>.</param>
    /// <returns>A new instance of <typeparamref name="TResult"/> with updated <see cref="StateResult.NextStateData"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
    public static TResult WithData<TResult, T>(TResult result, T data)
        where TResult : StateResult
    {
        ArgumentNullException.ThrowIfNull(result);
        return result with { NextStateData = JsonSerializer.Serialize(ToJsonElement(data)) };
    }

    private static JsonElement ToJsonElement(object? obj)
    {
        if (obj is null)
        {
            using var doc = JsonDocument.Parse("null");
            return doc.RootElement.Clone();
        }
        else
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            using var doc = JsonDocument.Parse(bytes);
            return doc.RootElement.Clone(); // Need cloning to make element live after disposing the document.
        }
    }
}
