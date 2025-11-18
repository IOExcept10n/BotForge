namespace BotForge.Modules;

/// <summary>
/// Represents an optional value, allowing the representation of a value that may or may not exist.
/// </summary>
/// <typeparam name="T">The type of the optional value.</typeparam>
public readonly record struct Optional<T>
{
    private readonly T _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Optional{T}"/> struct with a specified value.
    /// </summary>
    /// <param name="value">The value to encapsulate.</param>
    public Optional(T value)
    {
        _value = value!;
        HasValue = true;
    }

    /// <summary>
    /// Gets a value indicating whether the value exists.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets an instance representing no value.
    /// </summary>
    public static Optional<T> None => default;

    /// <summary>
    /// Creates an instance of <see cref="Optional{T}"/> containing a specified value.
    /// </summary>
    /// <param name="value">The value to encapsulate.</param>
    /// <returns>An instance of <see cref="Optional{T}"/> containing the specified value.</returns>
    public static Optional<T> Some(T value) => new(value);

    /// <summary>
    /// Retrieves the encapsulated value, throwing an exception if no value exists.
    /// </summary>
    /// <returns>The encapsulated value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no encapsulated value.</exception>
    public T ValueOrThrow()
    {
        if (!HasValue) throw new InvalidOperationException("Optional has no value.");
        return _value;
    }

    /// <summary>
    /// Gets the encapsulated value or the default value if no value exists.
    /// </summary>
    /// <returns>The encapsulated value, or the default value for type T.</returns>
    public T GetValueOrDefault() => HasValue ? _value : default!;

    /// <summary>
    /// Gets the encapsulated value or a specified default value if no value exists.
    /// </summary>
    /// <param name="defaultValue">The value to return if no value exists.</param>
    /// <returns>The encapsulated value, or the specified default value.</returns>
    public T GetValueOrDefault(T defaultValue) => HasValue ? _value : defaultValue;

    /// <summary>
    /// Tries to get the encapsulated value, returning a boolean indicating success.
    /// </summary>
    /// <param name="value">The output parameter receiving the encapsulated value if it exists.</param>
    /// <returns><see langword="true"/> if the optional value exists; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(out T value)
    {
        value = HasValue ? _value : default!;
        return HasValue;
    }

    /// <summary>
    /// Matches the optional value against two functions: one for when a value exists, and one for when it does not.
    /// </summary>
    /// <typeparam name="TResult">The result type of the match functions.</typeparam>
    /// <param name="onSome">The function to execute if a value exists.</param>
    /// <param name="onNone">The function to execute if no value exists.</param>
    /// <returns>The result of the matched function based on the existence of a value.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);
        return HasValue ? onSome(_value) : onNone();
    }

    /// <summary>
    /// Matches the optional value against two actions: one for when a value exists, and one for when it does not.
    /// </summary>
    /// <param name="onSome">The action to execute if a value exists.</param>
    /// <param name="onNone">The action to execute if no value exists.</param>
    public void Match(Action<T> onSome, Action onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);
        if (HasValue) onSome(_value); else onNone();
    }

    /// <summary>
    /// Returns a string representation of the optional value.
    /// </summary>
    /// <returns>A string indicating the state of the optional value.</returns>
    public override string ToString() => HasValue ? $"Some({_value})" : "None";

    // Operators
    /// <summary>
    /// Implicitly converts a value of type T to an instance of <see cref="Optional{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Optional<T>(T value) => new(value);

    /// <summary>
    /// Explicitly converts an instance of <see cref="Optional{T}"/> to its encapsulated value.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/> to convert.</param>
    /// <returns>The encapsulated value.</returns>
    public static explicit operator T(Optional<T> optional) => optional.ValueOrThrow();
}
