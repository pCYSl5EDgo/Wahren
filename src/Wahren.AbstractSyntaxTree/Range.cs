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
        var builder = PooledStringBuilder.Rent();
        builder.Append("Start: { Line: ");
        builder.Append(StartInclusive.Line);
        builder.Append(", Offset: ");
        builder.Append(StartInclusive.Offset);
        builder.Append(" },\nEnd: { Line: ");
        builder.Append(EndExclusive.Line);
        builder.Append(", Offset: ");
        builder.Append(EndExclusive.Offset);
        builder.Append("}");
        return builder.ToString();
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
