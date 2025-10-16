using BotForge.Messaging;

namespace BotForge.Fsm;

public record StateResult(string NextStateId, string NextStateData, ReplyContext? OverrideNextStateMessage = null)
{
    public StateResult(string nextStateId) : this(nextStateId, string.Empty, null) { }
}
