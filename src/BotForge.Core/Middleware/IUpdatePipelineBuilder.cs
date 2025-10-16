namespace BotForge.Middleware;

public interface IUpdatePipelineBuilder
{
    IUpdatePipelineBuilder Use(Func<UpdateDelegate, UpdateDelegate> middleware);

    IUpdatePipelineBuilder UseMiddleware<T>() where T : IUpdateMiddleware;

    UpdateDelegate Build(UpdateDelegate terminal);
}

public delegate Task UpdateDelegate(UpdateContext context, CancellationToken ct);
