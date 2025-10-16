using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotForge.Core.Messaging;

public interface IUpdateChannel
{
    event Func<IUpdate, CancellationToken, Task> OnUpdate;
}
