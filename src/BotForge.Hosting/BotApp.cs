using Microsoft.Extensions.Hosting;

namespace BotForge.Hosting;

/// <summary>
/// Provides methods for creating an instance of a bot builder in the BotForge framework.
/// </summary>
public static class BotApp
{
    /// <summary>
    /// Creates a new instance of <see cref="IBotBuilder"/> without any additional arguments.
    /// </summary>
    /// <returns>A new instance of <see cref="IBotBuilder"/> for building a bot application.</returns>
    public static IBotBuilder CreateBuilder() => new BotBuilder();

    /// <summary>
    /// Creates a new instance of <see cref="IBotBuilder"/> using command-line arguments.
    /// </summary>
    /// <param name="args">An array of strings representing command-line arguments provided to the application.</param>
    /// <returns>A new instance of <see cref="IBotBuilder"/> for building a bot application.</returns>
    public static IBotBuilder CreateBuilder(string[] args) => new BotBuilder(args);

    /// <summary>
    /// Creates a new instance of <see cref="IBotBuilder"/> using specified host application settings.
    /// </summary>
    /// <param name="settings">An instance of <see cref="HostApplicationBuilderSettings"/> containing configuration settings for the host.</param>
    /// <returns>A new instance of <see cref="IBotBuilder"/> for building a bot application.</returns>
    public static IBotBuilder CreateBuilder(HostApplicationBuilderSettings settings) => new BotBuilder(settings);
}
