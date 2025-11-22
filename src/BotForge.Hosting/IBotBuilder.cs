using Microsoft.Extensions.Hosting;

namespace BotForge.Hosting;

public interface IBotBuilder : IHostApplicationBuilder
{
    IHost Build();
}
