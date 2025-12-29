using Microsoft.Extensions.Hosting;

namespace BotForge.Hosting;

/// <summary>
/// Defines a builder for creating bot applications within the BotForge framework.
/// </summary>
public interface IBotBuilder : IHostApplicationBuilder
{
    /// <summary>
    /// Builds and returns an instance of <see cref="IHost"/> that represents the bot application.
    /// </summary>
    /// <returns>An instance of <see cref="IHost"/> for the bot application.</returns>
    IHost Build();
}
