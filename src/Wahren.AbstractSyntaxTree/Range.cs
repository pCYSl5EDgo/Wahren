namespace Wahren.AbstractSyntaxTree;

public struct Range : IEquatable<Range>
{
    public Position StartInclusive;
    public Position EndExclusive;

    public bool OneLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => StartAndEndLineAreSame || (StartInclusive.Line + 1 == EndExclusive.Line && EndExclusive.Offset == 0);
    }

    /// <summary>
    /// StartInclusive.Line == EndExclusive.Line
    /// </summary>
    public bool StartAndEndLineAreSame
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => StartInclusive.Line == EndExclusive.Line;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => StartAndEndLineAreSame && StartInclusive.Offset == EndExclusive.Offset;
    }

    /// <summary>
    /// EndExclusive.Offset == 0
    /// </summary>
    public bool IsEndAtLineEnd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => EndExclusive.Offset == 0;
    }

    public bool Equals(Range other) => Equals(ref other);

    public bool Equals(ref Range other) => StartInclusive.Equals(ref other.StartInclusive) && EndExclusive.Equals(ref other.EndExclusive);

    public override string ToString()
    {
        return $"StartLine: {StartInclusive.Line}, StartOffset: {StartInclusive.Offset}, EndLine: {EndExclusive.Line}, EndOffset: {EndExclusive.Offset}";
    }
}

public struct SingleLineRange
{
    public uint Line;
    public uint Offset;
    public uint Length;

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Length == 0;
        }
    }
}
