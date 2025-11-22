using System.Text.Json;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents data about model building, including type information and completion status.
/// </summary>
/// <param name="TypeName">The name of the type being built.</param>
/// <param name="PropertyName">The name of the property currently being bound.</param>
/// <param name="Model">The JSON element representation of the model.</param>
/// <param name="IsCompleted">Indicates whether the model binding process is complete.</param>
public readonly record struct ModelBuilderData(string TypeName, string? PropertyName, JsonElement Model, bool IsCompleted);
