using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BotForge.Core.Localization;
using BotForge.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Core.Fsm;

public static class StateExtensions
{
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

    public static bool Matches(this MessageStateContext ctx, ButtonLabel label)
    {
        if (ctx == null || label == null)
            return false;
        if (ctx.Message.Content is not TextMessageContent msg)
            return false;
        string text = label.Localize(ctx.Services.GetRequiredService<ILocalizationService>(), ctx.Message.From.Locale);
        return msg.Text == text;
    }

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
