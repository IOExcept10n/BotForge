using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules;

/// <summary>
/// Base class for bot modules providing common functionality for state management and input validation.
/// </summary>
public abstract class ModuleBase : IDisposable
{
    /// <summary>
    /// Localization key for invalid input error messages.
    /// </summary>
    public const string InvalidInputKey = "BaseModuleInvalidInput";

    /// <summary>
    /// The default name of the root state.
    /// </summary>
    public const string RootStateName = "root";

    /// <summary>
    /// Localization key for the root menu prompt.
    /// </summary>
    public const string SelectRootMenuKey = "ModuleSelectRootMenu";

    /// <summary>
    /// Key for unknown error messages.
    /// </summary>
    public const string UnknownErrorKey = "UnknownError";

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the ID of the root state associated with the module.
    /// </summary>
    public string RootStateId => $"{Name}:root";

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Syncrhonous variation of the <see cref="OnModuleRootAsync(SelectionStateContext, CancellationToken)"/>.
    /// </summary>
    /// <remarks>
    /// You can use this method in case when your root state is just a menu without any logic so it should just redirect user.
    /// </remarks>
    /// <param name="ctx">State context captured from <see cref="OnModuleRootAsync(SelectionStateContext, CancellationToken)"/>.</param>
    /// <returns>State transition to perform after handling this state.</returns>
    public virtual StateResult OnModuleRoot(SelectionStateContext ctx)
        => FailWithMessage(ctx, $"Error: Module does not override any of the root state methods. Consider overriding either {nameof(OnModuleRoot)} or {nameof(OnModuleRootAsync)}.");

    /// <summary>
    /// Handles the root state of the module.
    /// </summary>
    /// <param name="ctx">The context for the module state.</param>
    /// <param name="cancellationToken">An instance of the <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="StateResult"/> result.</returns>
    [Menu(SelectRootMenuKey, StateName = RootStateName)]
    public virtual Task<StateResult> OnModuleRootAsync(SelectionStateContext ctx, CancellationToken cancellationToken) => Task.FromResult(OnModuleRoot(ctx));

    internal async Task<ModelBindingBuilder.BindingResult> OnBindModelAsync(ModelBindContext ctx)
    {
        if (!ctx.TryGetData(out ModelBuilderData data))
        {
            var model = Activator.CreateInstance(ctx.Binding.RequestedModelType);
            if (model == null)
            {
                await ctx.ReplyAsync($"Model type '{ctx.Binding.RequestedModelType.FullName}' cannot be initialized using parameterless constructor.").ConfigureAwait(false);
                return new(false, []);
            }
            data = new(
                ctx.Binding.RequestedModelType.FullName!,
                ctx.Binding.ModelProperties[0].Name,
                ModelBindingBuilder.ToJsonElement(model),
                false);
        }
        var builder = ModelBindingBuilder.FromData(ctx.Binding, data, ctx.Services);
        var result = await OnUpdateModelPropertyAsync(ctx, builder).ConfigureAwait(false);

        foreach (var error in result.ValidationErrors)
        {
            if (error.ErrorMessage == null)
                continue;
            await ctx.ReplyAsync(error.ErrorMessage).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Handles failure and returns to the start state with an optional message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the failure action.</returns>
    protected internal static StateResult FailWithMessage(ModuleStateContext ctx, string? message)
        => FromStart(Localize(ctx, UnknownErrorKey), message);

    /// <summary>
    /// Retries the current operation with serialized data.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the retry action.</returns>
    protected internal static StateResult RetryWith<T>(ModuleStateContext ctx, T data, string? message = null)
        => Retry(ctx, JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Moves back to the previous state, passing optional state data and message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition back.</returns>
    protected internal StateResult Back(ModuleStateContext ctx, string? stateData, string? message)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        if (ctx.CurrentState.Id == RootStateId)
            return FromStart(stateData, message);
        if (ctx.Services.GetRequiredService<IRegistry<StateDefinition>>().TryGet(ctx.CurrentState.Id, out var current))
            return new(current.ParentStateId ?? RootStateId, stateData ?? string.Empty, new(message, null));
        return ToRoot(stateData, message);
    }

    /// <summary>
    /// Moves back to the previous state with an optional message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition back.</returns>
    protected internal StateResult Back(ModuleStateContext ctx, string? message)
        => Back(ctx, string.Empty, message);

    /// <summary>
    /// Moves back to the previous state without additional data or message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition back.</returns>
    protected internal StateResult Back(ModuleStateContext ctx)
        => Back(ctx, string.Empty, null);

    /// <summary>
    /// Moves back to the previous state, passing serialized data and an optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition back.</returns>
    protected internal StateResult BackWith<T>(ModuleStateContext ctx, T data, string? message = null)
        => Back(ctx, JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Handles invalid input and retries the current operation with an error message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <returns>A <see cref="StateResult"/> indicating the retry action for invalid input.</returns>
    protected internal virtual StateResult InvalidInput(ModuleStateContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return Retry(ctx, ctx.CurrentState.StateData, Localize(ctx, InvalidInputKey));
    }

    /// <summary>
    /// Handles failure and returns to the start state with optional error message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the failure action.</returns>
    protected static StateResult Fail(ModuleStateContext ctx, string? stateData, string? message)
        => FromStart(stateData ?? Localize(ctx, UnknownErrorKey), message);

    /// <summary>
    /// Handles failure, passing serialized data and optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the failure action.</returns>
    protected static StateResult FailWith<T>(ModuleStateContext ctx, T data, string? message = null)
        => Fail(ctx, JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Indicates the start state of the module with optional state data and message.
    /// </summary>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the start state.</returns>
    protected static StateResult FromStart(string? stateData, string? message)
        => new(StateRecord.StartStateId, stateData ?? string.Empty, new(message, null));

    /// <summary>
    /// Indicates the start state without additional data or message.
    /// </summary>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the start state.</returns>
    protected static StateResult FromStart()
        => FromStart(string.Empty, null);

    /// <summary>
    /// Indicates the start state with serialized data and optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the start state.</returns>
    protected static StateResult FromStartWith<T>(T data, string? message = null)
        => FromStart(JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Indicates the start state with an optional message.
    /// </summary>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the start state.</returns>
    protected static StateResult FromStartWithMessage(string? message)
        => FromStart(string.Empty, message);

    /// <summary>
    /// Localizes a string using the localization service based on the user's locale.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="text">The text to localize.</param>
    /// <returns>The localized string.</returns>
    protected static string Localize(ModuleStateContext ctx, string text)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return ctx.Services.GetRequiredService<ILocalizationService>().GetString(ctx.User.TargetLocale, text);
    }

    /// <summary>
    /// Localizes a string using the localization service with formatting support.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="text">The text to localize.</param>
    /// <param name="args">The formatting arguments.</param>
    /// <returns>The localized string with formatted arguments.</returns>
    protected static string Localize(ModuleStateContext ctx, string text, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return ctx.Services.GetRequiredService<ILocalizationService>().GetString(ctx.User.TargetLocale, text, args);
    }

    /// <summary>
    /// Retries the current operation with the passed state and message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the retry action.</returns>
    protected static StateResult Retry(ModuleStateContext ctx, string? stateData, string? message)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return new(ctx.CurrentState.Id, stateData ?? ctx.CurrentState.StateData, new(message, null));
    }

    /// <summary>
    /// Retries the current operation without additional data or message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <returns>A <see cref="StateResult"/> indicating the retry action.</returns>
    protected static StateResult Retry(ModuleStateContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return Retry(ctx, ctx.CurrentState.StateData, null);
    }

    /// <summary>
    /// Retries the current operation, passing only the message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the retry action.</returns>
    protected static StateResult RetryWithMessage(ModuleStateContext ctx, string? message)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        return Retry(ctx, ctx.CurrentState.StateData, message);
    }

    /// <summary>
    /// Transitions to a global state defined by a module with specified state key and optional state data and message.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the global state.</returns>
    protected static StateResult ToGlobalState<TModule>(ModuleStateContext ctx, string stateKey, string? stateData, string? message)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        string stateId = $"{typeof(TModule).Name}:{stateKey}";
        if (!ctx.Services.GetRequiredService<IRegistry<StateDefinition>>().TryGet(stateId, out _))
            throw new InvalidOperationException("Cannot move to the state that doesn't exist");
        return new(stateId, stateData ?? string.Empty, new(message, null));
    }

    /// <summary>
    /// Transitions to a global state defined by a module without additional data or message.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the global state.</returns>
    protected static StateResult ToGlobalState<TModule>(ModuleStateContext ctx, string stateKey)
        => ToGlobalState<TModule>(ctx, stateKey, string.Empty, null);

    /// <summary>
    /// Transitions to a global state defined by a module, passing serialized data and an optional message.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <typeparam name="TData">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the global state.</returns>
    protected static StateResult ToGlobalStateWith<TModule, TData>(ModuleStateContext ctx, string stateKey, TData data, string? message = null)
        => ToGlobalState<TModule>(ctx, stateKey, JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Transitions to a global state defined by a module with a specific message.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the global state.</returns>
    protected static StateResult ToGlobalStateWithMessage<TModule>(ModuleStateContext ctx, string stateKey, string? message)
        => ToGlobalState<TModule>(ctx, stateKey, string.Empty, message);

    /// <summary>
    /// Completes the operation and transitions to the root state with a specific message.
    /// </summary>
    /// <param name="message">The message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the completion.</returns>
    protected StateResult Completed(string message)
        => ToRootWithMessage(message);

    /// <summary>
    /// Disposes of resources used by the <see cref="ModuleBase"/> class.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    { }

    /// <summary>
    /// Handles failure and returns to the start state without additional data or message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <returns>A <see cref="StateResult"/> indicating the failure action.</returns>
    protected static StateResult Fail(ModuleStateContext ctx)
        => Fail(ctx, UnknownErrorKey, UnknownErrorKey);

    /// <summary>
    /// Transitions to the root state with optional state data and message.
    /// </summary>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the root state.</returns>
    protected StateResult ToRoot(string? stateData, string? message)
        => new(RootStateId, stateData ?? string.Empty, new(message, null));

    /// <summary>
    /// Transitions to the root state without additional data or message.
    /// </summary>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the root state.</returns>
    protected StateResult ToRoot()
        => ToRoot(string.Empty, null);

    /// <summary>
    /// Transitions to the root state with serialized data and an optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the root state.</returns>
    protected StateResult ToRootWith<T>(T data, string? message)
        => ToRoot(JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Transitions to the root state with an optional message.
    /// </summary>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the root state.</returns>
    protected StateResult ToRootWithMessage(string? message)
        => ToRoot(string.Empty, message);

    /// <summary>
    /// Transitions to a specific state defined by a state key, passing optional state data and message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="stateData">The state data to pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToState(ModuleStateContext ctx, string stateKey, string? stateData, string? message)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        string stateId = $"{Name}:{stateKey}";
        if (!ctx.Services.GetRequiredService<IRegistry<StateDefinition>>().TryGet(stateId, out _))
            throw new InvalidOperationException("Cannot move to the state that doesn't exist");
        return new(stateId, stateData ?? string.Empty, new(message, null));
    }

    /// <summary>
    /// Transitions to a specific state defined by a state key without additional data or message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToState(ModuleStateContext ctx, string stateKey)
        => ToState(ctx, stateKey, string.Empty, null);

    /// <summary>
    /// Transitions to a specific state using a delegate to specify the state.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateMethod">The delegate representing the state method.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToState(ModuleStateContext ctx, Delegate stateMethod)
    {
        ArgumentNullException.ThrowIfNull(stateMethod);
        return ToState(ctx, stateMethod.Method.Name);
    }

    /// <summary>
    /// Transitions to a specific state defined by a state key, passing serialized data and an optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToStateWith<T>(ModuleStateContext ctx, string stateKey, T data, string? message = null)
        => ToState(ctx, stateKey, JsonSerializer.Serialize(data), message);

    /// <summary>
    /// Transitions to a specific state using a delegate, passing serialized data and an optional message.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateMethod">The delegate representing the state method.</param>
    /// <param name="data">The data to serialize and pass.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToStateWith<T>(ModuleStateContext ctx, Delegate stateMethod, T data, string? message)
    {
        ArgumentNullException.ThrowIfNull(stateMethod);
        return ToStateWith(ctx, stateMethod.Method.Name, data, message);
    }

    /// <summary>
    /// Transitions to a specific state defined by a state key, passing an optional message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateKey">The key of the target state.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToStateWithMessage(ModuleStateContext ctx, string stateKey, string? message)
        => ToState(ctx, stateKey, string.Empty, message);

    /// <summary>
    /// Transitions to a specific state using a delegate, passing an optional message.
    /// </summary>
    /// <param name="ctx">The module state context.</param>
    /// <param name="stateMethod">The delegate representing the state method.</param>
    /// <param name="message">An optional message for the user.</param>
    /// <returns>A <see cref="StateResult"/> indicating the transition to the specified state.</returns>
    protected StateResult ToStateWithMessage(ModuleStateContext ctx, Delegate stateMethod, string? message)
    {
        ArgumentNullException.ThrowIfNull(stateMethod);
        return ToStateWithMessage(ctx, stateMethod.Method.Name, message);
    }

    /// <summary>
    /// Asynchronously validates a model and adds any validation errors to the provided collection.
    /// </summary>
    /// <param name="ctx">The model binding context.</param>
    /// <param name="bindingBuilder">The model binding builder.</param>
    /// <param name="errors">A collection to hold validation errors.</param>
    /// <returns>A task representing the validation result.</returns>
    protected virtual Task<bool> ValidateModelAsync(ModelBindContext ctx, ModelBindingBuilder bindingBuilder, ICollection<ValidationResult> errors) => Task.FromResult(true);

    /// <summary>
    /// Updates the model property based on the input from the user message.
    /// </summary>
    /// <param name="ctx">The model binding context.</param>
    /// <param name="bindingBuilder">The model binding builder.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="ModelBindingBuilder.BindingResult"/> result.</returns>
    private async Task<ModelBindingBuilder.BindingResult> OnUpdateModelPropertyAsync(ModelBindContext ctx, ModelBindingBuilder bindingBuilder)
    {
        if (ctx.Message.Content is not TextMessageContent msg)
            return new(false, [new(Localize(ctx, InvalidInputKey))]);

        var (ok, val) = bindingBuilder.InputProperty.Property.PropertyType == typeof(string) ?
            (true, msg.Text) :
            ParsableInvoker.TryParseAsObject(bindingBuilder.InputProperty.Property.PropertyType, msg.Text, ctx.User.TargetLocale);
        if (!ok)
            return new(false, [new(Localize(ctx, InvalidInputKey))]);

        var result = bindingBuilder.AppendValue(val);
        foreach (var error in result.ValidationErrors)
        {
            if (error.ErrorMessage == null)
                continue;
            error.ErrorMessage = Localize(ctx, error.ErrorMessage ?? string.Empty);
        }

        var customValidationResult = await ValidateModelAsync(ctx, bindingBuilder, result.ValidationErrors).ConfigureAwait(false);
        return result with { HasError = result.HasError || !customValidationResult };
    }
}
