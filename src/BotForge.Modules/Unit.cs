using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace BotForge.Modules;

/// <summary>
/// A struct representing an empty type, used as a placeholder instead of <see langword="void"/> for generic classes and methods.
/// Implements <see cref="IEquatable{T}"/>, <see cref="IComparable{T}"/>, and <see cref="IParsable{T}"/>.
/// </summary>
public readonly record struct Unit : IEquatable<Unit>, IComparable<Unit>, IEqualityOperators<Unit, Unit, bool>, IComparisonOperators<Unit, Unit, bool>, IUtf8SpanParsable<Unit>, IUtf8SpanFormattable, ISpanParsable<Unit>, ISpanFormattable
{
    /// <summary>
    /// Indicates the single instance of the Unit type.
    /// </summary>
    public static readonly Unit Value;

    /// <inheritdoc/>
    public static Unit Parse(string s, IFormatProvider? provider) => default;

    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Unit result) => true;

    /// <inheritdoc/>
    public int CompareTo(Unit other) => 0; // Since Unit is a singleton, all instances are equal.

    /// <inheritdoc/>
    public bool Equals(Unit other) => true; // Always equal because there's only one instance.

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => "Unit";

    /// <inheritdoc/>
    public override string ToString() => "Unit"; // Overrides the default ToString to provide meaningful output.

    /// <inheritdoc/>
    public override int GetHashCode() => 0; // Since Unit is a singleton, the hash code can be constant.

    /// <inheritdoc/>
    public static Unit Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => default;

    /// <inheritdoc/>
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, [MaybeNullWhen(false)] out Unit result) => true;

    /// <inheritdoc/>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        bytesWritten = 0;
        return true;
    }

    /// <inheritdoc/>
    public static Unit Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => default;

    /// <inheritdoc/>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Unit result) => true;

    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        return true;
    }

    /// <inheritdoc/>
    public static bool operator >(Unit left, Unit right) => false;

    /// <inheritdoc/>
    public static bool operator >=(Unit left, Unit right) => true;

    /// <inheritdoc/>
    public static bool operator <(Unit left, Unit right) => false;

    /// <inheritdoc/>
    public static bool operator <=(Unit left, Unit right) => true;
}

