namespace Wahren.AbstractSyntaxTree;

public struct Position : IEquatable<Position>
{
    public uint Line;
    public uint Offset;

    public bool TryIncrementLine(int count) => TryIncrement((uint)count);

    public bool TryIncrement(uint count)
    {
        if (Offset < count)
        {
            return false;
        }

        ++Line;
        Offset = 0;
        return true;
    }

    public bool Equals(Position other) => Equals(ref other);

    public bool Equals(ref Position other)
    {
        return Line == other.Line && Offset == other.Offset;
    }
}
