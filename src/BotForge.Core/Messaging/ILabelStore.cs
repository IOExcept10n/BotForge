using System.Globalization;

namespace BotForge.Core.Messaging;

public interface ILabelStore
{
    public ButtonLabel GetLabel(string key);
}
