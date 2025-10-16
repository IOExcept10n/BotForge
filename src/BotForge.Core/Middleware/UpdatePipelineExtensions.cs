namespace BotForge.Middleware;

public static class UpdatePipelineExtensions
{
    public static IUpdatePipelineBuilder WithMaxUsers(this IUpdatePipelineBuilder pipelineBuilder, int maxUsers)
    {
        ArgumentNullException.ThrowIfNull(pipelineBuilder);
        pipelineBuilder.Config = pipelineBuilder.Config with { MaxTrackedUsers = maxUsers };
        return pipelineBuilder;
    }

    public static IUpdatePipelineBuilder WithLockExpiration(this IUpdatePipelineBuilder pipelineBuilder, TimeSpan lockExpiration)
    {
        ArgumentNullException.ThrowIfNull(pipelineBuilder);
        pipelineBuilder.Config = pipelineBuilder.Config with { LockExpiration = lockExpiration };
        return pipelineBuilder;
    }
}
