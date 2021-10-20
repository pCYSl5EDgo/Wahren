namespace Wahren.AbstractSyntaxTree.Node;

public sealed class SoundNode
    : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public StringSpanKeyDictionary<Pair_NullableString_NullableIntElement> Others = new();

    public void Dispose()
    {
        Others.Dispose();
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Kind >= indexEqualToOrGreaterThan)
        {
            Kind += count;
        }

        if (BracketLeft >= indexEqualToOrGreaterThan)
        {
            BracketLeft += count;
        }

        if (BracketRight >= indexEqualToOrGreaterThan)
        {
            BracketRight += count;
        }

        foreach (var item in Others)
        {
            item?.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Kind >= indexEqualToOrGreaterThan)
        {
            Kind -= count;
        }

        if (BracketLeft >= indexEqualToOrGreaterThan)
        {
            BracketLeft -= count;
        }

        if (BracketRight >= indexEqualToOrGreaterThan)
        {
            BracketRight -= count;
        }

        foreach (var item in Others)
        {
            item?.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
