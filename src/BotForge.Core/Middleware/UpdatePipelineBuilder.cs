using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Middleware;

internal sealed class UpdatePipelineBuilder : IUpdatePipelineBuilder
{
    private readonly List<Func<UpdateHandler, UpdateHandler>> _middlewares = [];

    public UpdatePipelineConfig Config { get; set; } = new(10000, TimeSpan.FromSeconds(15));

    public UpdateHandler Build(UpdateHandler terminal)
    {
        var pipeline = terminal;
        for (int i = _middlewares.Count - 1; i >= 0; --i)
        {
            pipeline = _middlewares[i](terminal);
        }
        return pipeline;
    }

    public IUpdatePipelineBuilder Use(Func<UpdateHandler, UpdateHandler> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    public IUpdatePipelineBuilder UseMiddleware<T>() where T : IUpdateMiddleware
    {
        return Use(next => async (ctx, ct) =>
        {
            var middleware = ActivatorUtilities.CreateInstance<T>(ctx.Services);
            await middleware.InvokeAsync(ctx, () => next(ctx, ct), ct).ConfigureAwait(false);
        });
    }
}
