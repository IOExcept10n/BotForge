using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace BotForge.Hosting;

public class BotApp
{
    public static IBotBuilder CreateBuilder() => new BotBuilder();

    public static IBotBuilder CreateBuilder(string[] args) => new BotBuilder(args);

    public static IBotBuilder CreateBuilder(HostApplicationBuilderSettings settings) => new BotBuilder(settings);
}
