using System.Collections.Frozen;
using System.Reflection;

namespace BotForge.Core.Messaging;

internal sealed class LabelStore : ILabelStore
{
    private readonly FrozenDictionary<string, ButtonLabel> _labels = LoadLabelsFromAssembly(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

    public ButtonLabel GetLabel(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _labels.TryGetValue(key, out var label) ? label : key;
    }

    private static FrozenDictionary<string, ButtonLabel> LoadLabelsFromAssembly(Assembly asm)
    {
        var dict = new Dictionary<string, ButtonLabel>(StringComparer.OrdinalIgnoreCase);

        var typesWithAttr = asm.GetTypes()
            .Where(t => t.GetCustomAttribute<LabelStorageAttribute>(false) != null);

        foreach (var type in typesWithAttr)
        {
            // Public static fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var f in fields)
            {
                try
                {
                    var val = f.GetValue(null);
                    if (val != null)
                    {
                        var key = f.Name;
                        if (!dict.ContainsKey(key))
                            dict[key] = (ButtonLabel)val;
                    }
                }
                catch
                {
                }
            }

            // Public static properties without index parameters and with getter
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.GetIndexParameters().Length == 0 && p.GetGetMethod() != null);

            foreach (var p in props)
            {
                try
                {
                    var val = p.GetValue(null);
                    if (val != null)
                    {
                        var key = p.Name;
                        if (!dict.ContainsKey(key))
                            dict[key] = (ButtonLabel)val;
                    }
                }
                catch
                {
                }
            }
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}
