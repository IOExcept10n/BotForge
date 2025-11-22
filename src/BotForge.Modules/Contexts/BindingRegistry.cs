using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BotForge.Modules.Contexts;

internal class BindingRegistry : IRegistry<ModelBindingDescriptor>
{
    private readonly ConcurrentDictionary<string, ModelBindingDescriptor> _registry = [];

    public IEnumerator<ModelBindingDescriptor> GetEnumerator() => _registry.Values.GetEnumerator();

    public void Register(ModelBindingDescriptor instance) => _registry[instance.RequestedModelType.FullName ?? instance.RequestedModelType.Name] = instance;

    public bool TryGet(string key, [NotNullWhen(true)] out ModelBindingDescriptor? instance) => _registry.TryGetValue(key, out instance);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
