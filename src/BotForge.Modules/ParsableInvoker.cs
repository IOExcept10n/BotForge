using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace BotForge.Modules;

/// <summary>
/// Provides utitilies to parse strings as objects that implement <see cref="IParsable{TSelf}"/>.
/// </summary>
public static class ParsableInvoker
{
    private static readonly ConcurrentDictionary<Type, Func<string, IFormatProvider?, (bool success, object? value)>> _cache = [];

    /// <summary>
    /// Tries to parse a string as text.
    /// </summary>
    /// <param name="targetType">Target type to parse string representation for.</param>
    /// <param name="s">A string representation to an object to parse.</param>
    /// <param name="provider">An instance of the <see cref="IFormatProvider"/> to get numbers, dates and other data forms format info.</param>
    /// <returns>A tuple that provides info about whether the conversion succeded and value with results of the conversion.</returns>
    public static (bool success, object? value) TryParseAsObject(Type targetType, string s, IFormatProvider? provider = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentNullException.ThrowIfNull(s);

        var del = _cache.GetOrAdd(targetType, CreateInvoker);
        return del(s, provider);
    }

    private static Func<string, IFormatProvider?, (bool success, object? value)> CreateInvoker(Type targetType)
    {
        var parsableInterface = GetParsableInterface(targetType)
            ?? throw new InvalidOperationException($"Type {targetType.FullName} does not implement IParsable<T>.");

        var genericArg = parsableInterface.GetGenericArguments()[0];

        // Try to find a public static TryParse method with signature:
        // bool TryParse(string?, IFormatProvider?, out T)
        // Prefer method declared on the concrete type (targetType) or on the interface type.
        MethodInfo? tryParseMethod = FindTryParseMethod(targetType, genericArg)
            ?? FindTryParseMethod(parsableInterface, genericArg);

        if (tryParseMethod == null)
            throw new InvalidOperationException($"TryParse(string, IFormatProvider, out {genericArg.Name}) not found for {targetType.FullName}.");

        // Expression parameters
        var sParam = Expression.Parameter(typeof(string), "s");
        var providerParam = Expression.Parameter(typeof(IFormatProvider), "provider");

        // out variable of type T
        var outVar = Expression.Variable(genericArg, "outValue");

        // Build call: TryParse(sParam, providerParam, out outVar)
        var call = Expression.Call(tryParseMethod, sParam, providerParam, outVar);

        // Box outVar to object?
        var boxedOut = Expression.Convert(outVar, typeof(object));

        // Create tuple (bool, object?)
        var tupleType = typeof(ValueTuple<bool, object?>);
        var tupleCtor = tupleType.GetConstructor([typeof(bool), typeof(object)])
            ?? throw new InvalidOperationException("ValueTuple<bool, object?> constructor not found.");

        var newTuple = Expression.New(tupleCtor, call, boxedOut);

        var body = Expression.Block([outVar], newTuple);

        var lambda = Expression.Lambda<Func<string, IFormatProvider?, (bool, object?)>>(body, sParam, providerParam);
        return lambda.Compile();
    }

    private static MethodInfo? FindTryParseMethod(Type typeToSearch, Type resultType)
    {
        // Look for static public method named TryParse with 3 parameters: (string, IFormatProvider, out T) or (string, IFormatProvider?)
        var methods = typeToSearch.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(m => m.Name == "TryParse" && m.ReturnType == typeof(bool));

        foreach (var m in methods)
        {
            var parameters = m.GetParameters();
            if (parameters.Length != 3) continue;

            // first param string (or string?)
            if (parameters[0].ParameterType != typeof(string)) continue;

            // second param IFormatProvider
            var second = parameters[1].ParameterType;
            if (second != typeof(IFormatProvider)) continue;

            // third param must be out parameter of type T
            var third = parameters[2];
            if (!third.IsOut) continue;

            var paramElementType = third.ParameterType.GetElementType();
            if (paramElementType == null) continue;

            if (paramElementType == resultType)
                return m;
        }

        return null;
    }

    private static Type? GetParsableInterface(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IParsable<>))
            return type;

        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IParsable<>))
                return iface;
        }

        return null;
    }
}

