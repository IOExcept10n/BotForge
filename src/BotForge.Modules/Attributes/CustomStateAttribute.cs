using System;
using System.Collections.Generic;
using System.Text;

namespace BotForge.Modules.Attributes;

public sealed class CustomStateAttribute(string promptLocalizationKey) : FsmStateAttribute(promptLocalizationKey);
